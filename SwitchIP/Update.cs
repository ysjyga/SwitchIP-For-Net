using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace SwitchIP
{
    class Update
    {
        //使用WebClient下载
        public WebClient client = new WebClient();
        //当前版本
        public string localversion = null;
        //最新版本
        public string latesversion = null;
        //通知内容
        public string nnidtext = null;
        public Boolean updatestatus = true;

        public void CheckVersion()
        {
            NowVersion();
            if (!updatestatus)
            {
                return;
            }
            DownloadCheckUpdateXml();
            if (!updatestatus)
            {
                return;
            }
            LatestVersion();
            if (!updatestatus)
            {
                return;
            }
            DownloadInstall();
            if (!updatestatus)
            {
                return;
            }

        }
        //获取本地版本号
        public void NowVersion()
        {
            FileVersionInfo fv = FileVersionInfo.GetVersionInfo("SwitchIP.exe");
            localversion = fv.FileVersion;
        }
        /// <summary>
        /// 从服务器上获取最新的版本号
        /// </summary>
        public void DownloadCheckUpdateXml()
        {
            try
            {
                if (!Directory.Exists("Update"))
                {
                    Directory.CreateDirectory("Update");
                }
                //第一个参数是文件的地址,第二个参数是文件保存的路径文件名
                client.DownloadFile("http://switchip.svn.asia/Software/Update/SwitchIPUpdate.XML", @"Update\SwitchIPUpdate.XML");
                //client.DownloadFile("http://www.svn.asia", @"Update\SwitchIPUpdate.XML");
            }
            catch (WebException e)
            {
                nnidtext = "网络连接异常，无法连接更新服务器！\n" + e.ToString();
                MessageBox.Show(nnidtext);
                updatestatus = false;
                LogHelper.Error(this.GetType(), e.ToString());
                //Environment.Exit(0);
            }
            catch (Exception e)
            {
                nnidtext = "没有检测到更新";
                MessageBox.Show(nnidtext);
                updatestatus = false;
                LogHelper.Error(this.GetType(), e.ToString());
            }
        }

        /// <summary>
        /// 读取从服务器获取的最新版本号
        /// </summary>
        public void LatestVersion()
        {
            if (File.Exists(@"Update\SwitchIPUpdate.XML"))
            {
                XmlDocument doc = new XmlDocument();
                //加载要读取的XML
                doc.Load(@"Update\SwitchIPUpdate.XML");
                //获得根节点
                XmlElement WriteBook = doc.DocumentElement;

                //获得子节点 返回节点的集合
                XmlNodeList Update = WriteBook.ChildNodes;

                foreach (XmlNode item in Update)
                {
                    latesversion = item.InnerText;
                }
            }
            else if (!File.Exists(@"Update\SwitchIPUpdate.XML"))
            {
                nnidtext = "检查更新失败";
                MessageBox.Show(nnidtext);
                updatestatus = false;
                //Environment.Exit(0);
            }
        }

        /// <summary>
        /// 下载安装包
        /// </summary>
        public void DownloadInstall()
        {
            if (localversion == latesversion)
            {
                nnidtext = "恭喜您，已经更新到最新版本！";
                MessageBox.Show(nnidtext);
                updatestatus = false;
            }
            else if (localversion != latesversion && File.Exists(@"Update\SwitchIPUpdate.XML"))
            {
                nnidtext = "发现新版本，即将下载更新补丁";
                MessageBox.Show(nnidtext);
                client.DownloadFile("http://switchip.svn.asia/Software/Update/SwitchIP.exe", @"Update\SwitchIP.exe");
                if (File.Exists(@"Update\SwitchIP.exe"))
                {
                    InstallandDelete();//这里调用安装的类
                }
                else if (!File.Exists(@"Update\SwitchIP.exe"))
                {
                    //如果一次没有下载成功，则检查三次
                    for (int i = 1; i < 3; i++)
                    {
                        client.DownloadFile("http://switchip.svn.asia/Software/Update/SwitchIP.exe", @"Update\SwitchIP.exe");
                    }
                    nnidtext = "下载失败，请检查您的网络连接是否正常";
                    MessageBox.Show(nnidtext);
                    updatestatus = false;
                    //Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// 安装及删除
        /// </summary>
        private void InstallandDelete()
        {
            FileInfo file = new FileInfo(@"Update\SwitchIP.exe");
            if (file.Exists)
            {
                FileInfo file_ = new FileInfo(@"SwitchIP_bak.exe");
                if (file_.Exists)
                {
                    file_.Delete();
                }
                File.Delete(@"Update\SwitchIPUpdate.XML");
                //File.Move(@"Update\SwitchIPUpdate.XML", "");
                // true is overwrite
                File.Move(@"Update\SwitchIP.exe", @"SwitchIP_bak.exe"); 

            }
            nnidtext = "更新成功！";
            MessageBox.Show(nnidtext);
            //启动程序
            Process.Start(@"SwitchIP_bak.exe");
            File.Move(@"SwitchIP.exe", @"SwitchIP_old.exe");
            //安装前关闭正在运行的程序            
            File.Move(@"SwitchIP_bak.exe", @"SwitchIP.exe");
            
            File.Move(@"SwitchIP_old.exe", @"SwitchIP_bak.exe");
           
            JudgeInstall();
            KillProgram();
        }

        /// <summary>
        /// 判断安装进程是否存在
        /// </summary>
        public void JudgeInstall()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == "SwitchIP")
                {
                    //process.Kill();
                    File.Delete(@"Update\SwitchIP.exe");
                    File.Delete(@"Update\SwitchIPUpdate.XML");
                    process.Kill();

                }
                else
                {
                    File.Delete(@"Update\SwitchIP.exe");
                    File.Delete(@"Update\SwitchIPUpdate.XML");
                    return;
                }
            }
        }

        /// <summary>
        /// 结束程序
        /// </summary>
        public void KillProgram()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                //如果程序启动了，则杀死
                if (process.ProcessName == "SwitchIP")
                {
                    process.Kill();
                }
            }
        }
    }
}