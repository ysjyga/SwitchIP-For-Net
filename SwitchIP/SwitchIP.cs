using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SwitchIP
{

    public partial class SwitchIP : Form
    {
        //设置配置文件路径 程序路径 + 配置文件名
        string file = INIOperation.IniFilePath();

        public SwitchIP()
        {
            InitializeComponent();
            InitTree_FromConfig();
            Init();
        }
        private void Init()
        {
            LogHelper.Debug(this.GetType(), "开始读取网络适配器信息。。。");
            List<string> NetWork_Name = new List<string>();
            NetWorkOperation NetWork = new NetWorkOperation();
            NetWork_Name = NetWork.Query_NetWork_Name();
            for (int i = 0; i < NetWork_Name.Count; i++)
            {
                comboBox1.Items.Insert(0, NetWork_Name[i]);
                if (NetWork_Name[i].IndexOf("连接") > 0)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(NetWork_Name[i]);
                }
            }
            LogHelper.Debug(this.GetType(), "完成读取网络适配器信息。。。");
        }

        private void InitTree_FromConfig()
        {
            //TreeNode node = new TreeNode("base1");
            //TreeNode node2 = new TreeNode("base2");
            //treeView1.Nodes.Add(node);
            //treeView1.Nodes.Add(node2);
            //node.Nodes.Add("a");
            //node.Nodes.Add("b");
            //node.Nodes.Add("c"); 
            //node2.Nodes.Add("d");
            //node2.Nodes.Add("e");
            //node2.Nodes.Add("f");
            LogHelper.Debug(this.GetType(), "开始读取配置文件中配置方案信息。。。");
            string sectionsname = "";
            string groupname = "";
            string[] sections = INIOperation.INIGetAllSectionNames(file);
            List<string> group = new List<string>();
            List<TreeNode> group_node = new List<TreeNode>();
            TreeNode node = null;
            int count;
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsname = sections[i];
                LogHelper.Debug(this.GetType(), "设置父节点名称为“当前配置”。。。");
                groupname = INIOperation.INIGetStringValue(file, sectionsname, "group", "当前配置");
                if (group.IndexOf(groupname) < 0)
                {
                    group.Add(groupname);
                    node = new TreeNode(groupname);
                    treeView1.Nodes.Add(node);
                    group_node.Add(node);
                }
                count = group.IndexOf(groupname);
                group_node[count].Nodes.Add(sectionsname);
            }
            LogHelper.Debug(this.GetType(), "完成读取配置文件中配置方案信息。。。");
            treeView1.Sort();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Enabled = false;
            textBox2.Clear();
            textBox2.Enabled = false;
            textBox3.Clear();
            textBox3.Enabled = false;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Clear();
            textBox4.Enabled = false;
            textBox5.Clear();
            textBox5.Enabled = false;
        }

        private void RadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = true;
            textBox5.Enabled = true;
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.treeView1.ExpandAll();
        }


        //设置树单选,就是只能有一个树节点被选中
        private void SetNodeCheckStatus(TreeNode tn, TreeNode node)
        {
            if (tn == null)
                return;
            if (tn != node)
            {
                tn.Checked = false;
            }
            // Check children nodes
            foreach (TreeNode tnChild in tn.Nodes)
            {
                if (tnChild != node)
                {
                    tnChild.Checked = false;
                }
                SetNodeCheckStatus(tnChild, node);
            }
        }
        private String m_NodeName = null;
        //在树节点被选中后触发
        private void TreeView1_AfterCheacked(object sender, TreeViewEventArgs e)
        {
            //过滤不是鼠标选中的其它事件，防止死循环
            if (e.Action != TreeViewAction.Unknown)
            {
                //Event call by mouse or key-press
                foreach (TreeNode tnChild in treeView1.Nodes)
                    SetNodeCheckStatus(tnChild, e.Node);
                string sName = e.Node.Text;
            }
        }
        //获得选择节点
        private void GetSelectNode(TreeNode tn)
        {
            if (tn == null)
                return;
            if (tn.Checked == true)
            {
                m_NodeName = tn.Text;
                return;
            }
            // Check children nodes
            foreach (TreeNode tnChild in tn.Nodes)
            {
                GetSelectNode(tnChild);
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            //TreeNode node = null;
            foreach (TreeNode tnChild in treeView1.Nodes)
            {
                GetSelectNode(tnChild);
            }
            string sName = m_NodeName;
        }
        //选择树的节点，触发事件
        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            //选择树的节点并点击右键，触发事件
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                if (CurrentNode != null && true == CurrentNode.Checked)//判断你点的是不是一个节点
                    switch (CurrentNode.Name)//根据不同节点显示不同的右键菜单，当然你可以让它显示一样的菜单
                    {
                        case "":
                            //右键菜单
                            CurrentNode.ContextMenuStrip = contextMenuStrip1;
                            break;
                        default:
                            break;
                    }
                treeView1.SelectedNode = CurrentNode;//选中这个节点
            }
            //选择树的节点并点击左键，触发事件
            if (e.Button == MouseButtons.Left)
            {
                if ((sender as TreeView) != null)
                {
                    treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
                    if (treeView1.SelectedNode != null)
                    {
                        string sectionname = treeView1.SelectedNode.Text;
                        //设置点击当前配置时自动读取当前使用的IP设置信息
                        if (sectionname == "当前配置")
                        {
                            LogHelper.Debug(this.GetType(), "“当前配置”获取IP配置信息。。。");
                            NetWorkOperation NetWork = new NetWorkOperation();
                            string NetWork_Name = comboBox1.SelectedItem.ToString();
                            string[] arr = NetWork_Name.Split('|');

                            int DhcpCount = NetWork.Query_NetWork_Dhcp_Status(arr[2]);

                            if (DhcpCount > 0)
                            {
                                radioButton1.Checked = true;
                                radioButton2.Checked = false;
                                radioButton3.Checked = true;
                                radioButton4.Checked = false;
                                textBox1.Enabled = false;
                                textBox2.Enabled = false;
                                textBox3.Enabled = false;
                                textBox4.Enabled = false;
                                textBox5.Enabled = false;
                            }
                            else
                            {
                                List<string> list = NetWork.GetNetWork(arr[2]);
                                radioButton1.Checked = false;
                                radioButton2.Checked = true;
                                radioButton3.Checked = false;
                                radioButton4.Checked = true;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                textBox3.Enabled = true;
                                textBox4.Enabled = true;
                                textBox5.Enabled = true;
                                textBox1.Text = list[0];
                                textBox2.Text = list[1];
                                textBox3.Text = list[2];
                                textBox4.Text = list[1];
                                textBox5.Text = list[2];
                            }
                            return;
                        }

                        LogHelper.Debug(this.GetType(), "读取配置文件信息到IP栏并展示。。。");
                        string ipradio = INIOperation.INIGetStringValue(file, sectionname, "ipradio", "");
                        string dnsradio = INIOperation.INIGetStringValue(file, sectionname, "dnsradio", "");
                        string iplist = INIOperation.INIGetStringValue(file, sectionname, "iplist", "");
                        string masklist = INIOperation.INIGetStringValue(file, sectionname, "masklist", "");
                        string gatewaylist = INIOperation.INIGetStringValue(file, sectionname, "gatewaylist", "");
                        string preferreddnslist = INIOperation.INIGetStringValue(file, sectionname, "preferreddnslist", "");
                        string optionaldnslist = INIOperation.INIGetStringValue(file, sectionname, "optionaldnslist", "");
                        string networkdescription = INIOperation.INIGetStringValue(file, sectionname, "networkdescription", "");
                        string NetWorkName = "";
                        for (int i = 0; i < comboBox1.Items.Count; i++)
                        {
                            NetWorkName = comboBox1.Items[i].ToString();
                            string[] arr = NetWorkName.Split('|');
                            if (arr[2] == networkdescription)
                            {
                                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(NetWorkName);
                                break;
                            }
                        }

                        if (ipradio != "")
                        {
                            if (ipradio == "1")
                            {
                                radioButton1.Checked = false;
                                radioButton2.Checked = true;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                textBox3.Enabled = true;
                            }
                            else
                            {
                                radioButton1.Checked = true;
                                radioButton2.Checked = false;
                                textBox1.Enabled = false;
                                textBox2.Enabled = false;
                                textBox3.Enabled = false;
                            }
                            if (dnsradio == "1")
                            {
                                radioButton3.Checked = false;
                                radioButton4.Checked = true;
                                textBox4.Enabled = true;
                                textBox5.Enabled = true;
                            }
                            else
                            {
                                radioButton3.Checked = true;
                                radioButton4.Checked = false;
                                textBox4.Enabled = false;
                                textBox5.Enabled = false;
                            }
                            textBox1.Text = iplist;
                            textBox2.Text = masklist;
                            textBox3.Text = gatewaylist;
                            textBox4.Text = preferreddnslist;
                            textBox5.Text = optionaldnslist;
                        }
                    }
                }
            }
        }

        //右键添加节点
        private void ToolStripMenuItem0_Click(object sender, EventArgs e)
        {
            //在Tree选择节点的同一级添加节点
            treeView1.LabelEdit = true;
            TreeNode CurrentNode = null;
            try
            {
                CurrentNode = treeView1.SelectedNode.Nodes.Add("Node1");
                //更新选择节点
                treeView1.SelectedNode.Checked = false;
                CurrentNode.Checked = true;
                treeView1.ExpandAll();
                //使添加的树节点处于可编辑的状态
                CurrentNode.BeginEdit();
            }
            catch
            //(Exception k)
            {
                //MessageBox.Show("没有选中节点！");
                TreeNode node = new TreeNode("新建组");
                treeView1.Nodes.Add(node);
                treeView1.ExpandAll();
                node.BeginEdit();
            }


        }
        //右键设置节点可以重命名
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //窗体的LabelEdir为false，因此每次要BeginEdit时都要先自LabelEdit为true
            treeView1.LabelEdit = true;
            treeView1.SelectedNode.BeginEdit();
        }
        //右键删除节点
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode.Remove();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string txtIP = textBox1.Text;
            string txtSubMark = textBox2.Text;
            string txtGateWay = textBox3.Text;
            string txtDNS1 = textBox4.Text;
            string txtDNS2 = textBox5.Text;
            string NetWork_Name = "";

            NetWorkOperation NetWork = new NetWorkOperation();

            string[] ip = new string[] { txtIP.Trim() };
            string[] SubMark = new string[] { txtSubMark.Trim() };
            string[] GateWay = new string[] { txtGateWay.Trim() };
            string[] DNS = new string[] { txtDNS1.Trim(), txtDNS2.Trim() };

            NetWork_Name = comboBox1.SelectedItem.ToString();
            string[] arr = NetWork_Name.Split('|');

            LogHelper.Debug(this.GetType(), "开始进行IP切换。。。");
            Boolean Str_return = true;
            if (radioButton2.Checked)
            {
                Str_return = NetWork.IP_Check(txtIP, txtSubMark, txtGateWay);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "IP地址格式检测 Str_return = " + Str_return);
                Str_return = NetWork.SetIpInfo(ip, SubMark, GateWay, arr[2]);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "设置IP Str_return = " + Str_return);
            }
            else
            {
                Str_return = NetWork.EnableIpDHCP(arr[2]);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "设置IPDHCP Str_return = " + Str_return);
            }
            if (radioButton4.Checked)
            {
                Str_return = NetWork.Dns_Check(txtDNS1, txtDNS2);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "DNS地址格式检测 Str_return = " + Str_return);
                Str_return = NetWork.SetDnsInfo(DNS, arr[2]);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "设置DNS Str_return = " + Str_return);
            }
            else
            {
                Str_return = NetWork.EnableDnsDHCP(arr[2]);
                if (!Str_return)
                {
                    return;
                }
                LogHelper.Debug(this.GetType(), "设置IPDHCP Str_return = " + Str_return);
            }

            MessageBox.Show("IP切换成功！");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();//退出应用程序 
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string str;
            string txtIP = textBox1.Text;
            string txtSubMark = textBox2.Text;
            string txtGateWay = textBox3.Text;
            string txtDNS1 = textBox4.Text;
            string txtDNS2 = textBox5.Text;
            //string NetworkDescription = comboBox1.Text;
            string NetWork_Name = comboBox1.SelectedItem.ToString();
            string[] arr = NetWork_Name.Split('|');
            Boolean Str_return = true;
            string txtIpRadio, txtDnsRadio;
            NetWorkOperation NetWork = new NetWorkOperation();

            str = Interaction.InputBox("请输入方案名称！\n\n如方案名称已存在，则会覆盖以前的方案！", "新方案", "", -1, -1);
            if (str == "")
            {
                MessageBox.Show("方案名称不能为空！");
                return;
            }

            if (radioButton2.Checked)
            {
                txtIpRadio = "1";
                Str_return = NetWork.IP_Check(txtIP, txtSubMark, txtGateWay);
                if (!Str_return)
                {
                    return;
                }
                INIOperation.INIWriteValue(file, str, "networkdescription", arr[2]);
                INIOperation.INIWriteValue(file, str, "ipradio", txtIpRadio);
                INIOperation.INIWriteValue(file, str, "iplist", txtIP);
                INIOperation.INIWriteValue(file, str, "masklist", txtSubMark);
                INIOperation.INIWriteValue(file, str, "gatewaylist", txtGateWay);
            }
            else
            {
                txtIpRadio = "0";
                INIOperation.INIWriteValue(file, str, "networkdescription", arr[2]);
                INIOperation.INIWriteValue(file, str, "ipradio", txtIpRadio);
            }
            if (radioButton4.Checked)
            {
                txtDnsRadio = "1";
                Str_return = NetWork.IP_Check(txtIP, txtSubMark, txtGateWay);
                if (!Str_return)
                {
                    return;
                }

                INIOperation.INIWriteValue(file, str, "dnsradio", txtDnsRadio);
                INIOperation.INIWriteValue(file, str, "preferreddnslist", txtDNS1);
                INIOperation.INIWriteValue(file, str, "optionaldnslist", txtDNS2);
            }
            else
            {
                txtDnsRadio = "0";
                INIOperation.INIWriteValue(file, str, "dnsradio", txtDnsRadio);
            }
            treeView1.Nodes.Clear();
            InitTree_FromConfig();
            this.treeView1.ExpandAll();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string sectionname;
            if (treeView1.SelectedNode != null)
            {
                sectionname = treeView1.SelectedNode.Text;
                treeView1.SelectedNode.Remove();
                INIOperation.INIDeleteSection(file, sectionname);
                treeView1.SelectedNode = null;
            }
            else
            {
                MessageBox.Show("请选择要删除的方案名称！");
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileVersionInfo fv = FileVersionInfo.GetVersionInfo("SwitchIP.exe");
            String localversion = fv.FileVersion;
            MessageBox.Show("软件名称：SwitchIP - 切换IP工具【By TanBin】\n软件版本：SWIP-" + localversion + " \n官方网站：http://switchip.svn.asia\n授权信息：Copyright@2018 SwitchIP, Inc.All Rights Reserved\n版权说明：SwitchIP 用于非商业使用是免费的。您也可以在无报酬的工作中任意使用且没有时间限制，任何形式的商业使用、工作使用、公司笔记本电脑…等都需要取得SwitchIP Pro授权");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string NetWork_Name = comboBox1.SelectedItem.ToString();
            string[] arr = NetWork_Name.Split('|');
            textBox6.Text = arr[4];
        }

        private void CheckUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //版本更新检测
            Update update = new Update();
            update.CheckVersion();
        }

        private void WebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Process.Start("http://www.switchip.cn");
        }

        private void WriteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteLogToolStripMenuItem.Checked = !WriteLogToolStripMenuItem.Checked;
            if (WriteLogToolStripMenuItem.Checked)
            {
                LogHelper.IsWriteLog_("Y");
            }
            else
            {
                LogHelper.IsWriteLog_("N");
            }
        }
    }
}
