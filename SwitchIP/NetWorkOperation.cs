using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace SwitchIP
{
    class NetWorkOperation
    {
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();//获取本地计算机上网络接口的对象

        //获取本地计算机上网络接口名称
        public List<string> Query_NetWork_Name()
        {
            NetWork_Info();
            List<string> NetWork_Name = new List<string>();
            string NetWork_mac = null;
            string NetWork_Status = null;
            string NetWork_Type = null;
            string NetworkInterfaceType = null;
            foreach (NetworkInterface adapter in adapters)
            {
                NetWork_mac = adapter.GetPhysicalAddress().ToString();
                if (NetWork_mac.Length > 0 && NetWork_mac.Length < 13)
                {
                    // 格式化显示MAC地址                
                    PhysicalAddress pa = adapter.GetPhysicalAddress();//获取适配器的媒体访问（MAC）地址
                    byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                    StringBuilder PhysicalAddress = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        PhysicalAddress.Append(bytes[i].ToString("X2"));//以十六进制格式化
                        if (i != bytes.Length - 1)
                        {
                            PhysicalAddress.Append("-");
                        }
                    }
                    NetworkInterfaceType = adapter.NetworkInterfaceType.ToString();
                    if (NetworkInterfaceType.IndexOf("Wireless") > -1)
                    {
                        NetWork_Type = "无线网卡";
                    }
                    else if (NetworkInterfaceType.IndexOf("Ethernet") > -1)
                    {
                        NetWork_Type = "有线网卡";
                    }
                    NetWork_Status = adapter.OperationalStatus.ToString();
                    NetWork_Status = (NetWork_Status == "Up") ? "已连接" : "已断开";
                    NetWork_Name.Add(adapter.Name + "|" + NetWork_Type + "|" + adapter.Description + "|" + NetWork_Status + "|" + PhysicalAddress);
                }
            }
            return NetWork_Name;
        }
        public int Query_NetWork_Dhcp_Status(string NetWorkName)
        {
            int NetWork_Dhcp_Status = 1;
            foreach (NetworkInterface adapter in adapters)
            {

                if (adapter.Description == NetWorkName)
                {
                    NetWork_Dhcp_Status = adapter.GetIPProperties().DhcpServerAddresses.Count;
                }
            }
            return NetWork_Dhcp_Status;
        }
        //获取本地计算机上网卡信息
        public string NetWork_Info()
        {
            Console.WriteLine("适配器个数：" + adapters.Length);
            Console.WriteLine();
            foreach (NetworkInterface adapter in adapters)
            {
                Console.WriteLine("描述：" + adapter.Description);
                Console.WriteLine("标识符：" + adapter.Id);
                Console.WriteLine("名称：" + adapter.Name);
                Console.WriteLine("类型：" + adapter.NetworkInterfaceType);
                Console.WriteLine("速度：" + adapter.Speed * 0.001 * 0.001 + "M");
                Console.WriteLine("操作状态：" + adapter.OperationalStatus);
                Console.WriteLine("MAC 地址：" + adapter.GetPhysicalAddress());
                LogHelper.Debug(this.GetType(), "描述：" + adapter.Description);
                LogHelper.Debug(this.GetType(), "标识符：" + adapter.Id);
                LogHelper.Debug(this.GetType(), "名称：" + adapter.Name);
                LogHelper.Debug(this.GetType(), "类型：" + adapter.NetworkInterfaceType);
                LogHelper.Debug(this.GetType(), "速度：" + adapter.Speed * 0.001 * 0.001 + "M");
                LogHelper.Debug(this.GetType(), "操作状态：" + adapter.OperationalStatus);
                LogHelper.Debug(this.GetType(), "MAC 地址：" + adapter.GetPhysicalAddress());

                // 格式化显示MAC地址                
                PhysicalAddress pa = adapter.GetPhysicalAddress();//获取适配器的媒体访问（MAC）地址
                byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("X2"));//以十六进制格式化
                    if (i != bytes.Length - 1)
                    {
                        sb.Append("-");
                    }
                }
                Console.WriteLine("MAC 地址：" + sb);
                LogHelper.Debug(this.GetType(), "MAC 地址：" + sb);
                Console.WriteLine();
            }
            //Console.ReadKey();
            return null;
        }
        #region 设置ip信息到网卡  
        public Boolean IP_Check(string txtIP, string txtSubMark, string txtGateWay)
        {

            if (!IsIpaddress(txtIP.Trim()))
            {
                MessageBox.Show("【IP地址】 格式不正确！");
                return false;
            }
            if (!IsIpaddress(txtSubMark.Trim()))
            {
                MessageBox.Show("【子网掩码】 格式不正确！");
                return false;
            }
            if (!IsIpaddress(txtGateWay.Trim()))
            {
                MessageBox.Show("【网关】 格式不正确！");
                return false;
            }
            return true;
        }
        #endregion
        public Boolean Dns_Check(string txtDNS1, string txtDNS2)
        {
            if (!IsIpaddress(txtDNS1.Trim()))
            {
                MessageBox.Show("【首选DNS】 格式不正确！");
                return false;
            }
            if (!IsIpaddress(txtDNS2.Trim()))
            {
                MessageBox.Show("【备选DNS】 格式不正确！");
                return false;
            }
            return true;
        }
        public Boolean SetIpInfo(string[] ip, string[] SubMark, string[] GateWay, string NetWorkName)
        {
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string str = "";
            foreach (ManagementObject mo in moc)
            {
                if (mo["caption"].ToString().IndexOf(NetWorkName) > 0)
                {
                    if (mo["IPEnabled"].ToString() == "False")
                    {
                        MessageBox.Show("您所选择的网络适配器未连接网线或未连接Wifi,不能进行IP切换！");
                        return false;
                    }
                    inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = ip;//ip地址  
                    inPar["SubnetMask"] = SubMark; //子网掩码   
                    outPar = mo.InvokeMethod("EnableStatic", inPar, null);//执行  
                    str = outPar["returnvalue"].ToString(); //获取操作设置IP的返回值， 可根据返回值去确认IP是否设置成功。 0或1表示成功 
                    LogHelper.Debug(this.GetType(), "设置ip地址、子网掩码返回结果：str = " + str);
                    if (str != "0" && str != "1")
                    {
                        MessageBox.Show("IP切换失败！\n失败原因可能为：\n1.IP冲突，请更换IP后再试！");
                        return false;
                    }
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = GateWay; //设置网关地址 1.网关;2.备用网关  
                    outPar = mo.InvokeMethod("SetGateways", inPar, null);//执行  
                    str = outPar["returnvalue"].ToString(); //获取操作设置IP的返回值， 可根据返回值去确认IP是否设置成功。 0或1表示成功   
                    LogHelper.Debug(this.GetType(), "设置网关地址返回结果：str = " + str);
                    break; //只设置一张网卡，不能多张。  
                }
            }
            return (str == "0" || str == "1") ? true : false;

        }
        public Boolean SetDnsInfo(string[] DNS, string NetWorkName)
        {
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string str = "";
            foreach (ManagementObject mo in moc)
            {
                if (mo["caption"].ToString().IndexOf(NetWorkName) > 0)
                {
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inPar["DNSServerSearchOrder"] = DNS; //设置DNS  1.DNS 2.备用DNS  
                    outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);// 执行  
                    str = outPar["returnvalue"].ToString(); //获取操作设置IP的返回值， 可根据返回值去确认IP是否设置成功。 0或1表示成功  
                    LogHelper.Debug(this.GetType(), "设置DNS返回结果：str = " + str);
                    break; //只设置一张网卡，不能多张。  
                }
            }
            return (str == "0" || str == "1") ? true : false;
        }
        // 启用IP DHCP服务器  
        public Boolean EnableIpDHCP(string NetWorkName)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            string str = "";
            foreach (ManagementObject mo in moc)
            {
                if (mo["caption"].ToString().IndexOf(NetWorkName) > 0)
                {
                    if (mo["IPEnabled"].ToString() == "False")
                    {
                        MessageBox.Show("您所选择的网络适配器未连接网线或未连接Wifi,不能进行IP切换！");
                        return false;
                    }
                    //开启IP DHCP  
                    str = mo.InvokeMethod("EnableDHCP", null).ToString();
                    LogHelper.Debug(this.GetType(), "设置IP DHCP返回结果：str = " + str);
                    break; //只设置一张网卡，不能多张。  
                }
            }
            if (str == "0" || str == "1")
            {
                return true;
            }
            else
            {
                MessageBox.Show("IP切换失败 - DHCP！");
                return false;
            }
        }
        // 启用DNS DHCP服务器  
        public Boolean EnableDnsDHCP(string NetWorkName)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            string str = "";
            foreach (ManagementObject mo in moc)
            {
                if (mo["caption"].ToString().IndexOf(NetWorkName) > 0)
                {
                    //重置DNS为空  
                    str = mo.InvokeMethod("SetDNSServerSearchOrder", null).ToString();
                    LogHelper.Debug(this.GetType(), "重置DNS为空返回结果：str = " + str);
                    break; //只设置一张网卡，不能多张。  
                }
            }
            return (str == "0" || str == "1") ? true : false;
        }
        #region 判断是否是正确的ip地址  
        // 判断是否是正确的ip地址         
        protected bool IsIpaddress(string ipaddress)
        {
            string[] nums = ipaddress.Split('.');
            if (nums.Length != 4) return false;
            foreach (string num in nums)
            {
                if (Convert.ToInt32(num) < 0 || Convert.ToInt32(num) > 255) return false;
            }
            return true;
        }
        #endregion
        //获取本机IP、DNS信息
        public List<string> GetNetWork(string NetWorkName)
        {

            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            List<string> list = new List<string>();
            foreach (ManagementObject mo in moc)
            {
                if (mo["caption"].ToString().IndexOf(NetWorkName) > 0)
                {
                    //重置DNS为空  
                    string strIP, strSubnet, strGateway, strDNS, strDNS1;
                    if (mo["IPAddress"] == null)
                    {
                        strIP = "";
                        strSubnet = "";
                        strGateway = "";
                    }
                    else
                    {
                        strIP = (mo["IPAddress"] as String[])[0];
                        strSubnet = (mo["IPSubnet"] as String[])[0];
                        strGateway = (mo["DefaultIPGateway"] as String[])[0];
                    }
                    if (mo["DNSServerSearchOrder"] == null)
                    {
                        strDNS = "";
                        strDNS1 = "";
                    }
                    else
                    {
                        strDNS = (mo["DNSServerSearchOrder"] as String[])[0];
                        if ((mo["DNSServerSearchOrder"] as String[]).Length > 1)
                        {
                            strDNS1 = (mo["DNSServerSearchOrder"] as String[])[1];
                        }
                        else
                        {
                            strDNS1 = "";
                        }
                    }
                    list.Add(strIP);
                    list.Add(strSubnet);
                    list.Add(strGateway);
                    list.Add(strDNS);
                    list.Add(strDNS1);
                    break; //只设置一张网卡，不能多张。  
                }
            }
            return list;
        }

    }
}
