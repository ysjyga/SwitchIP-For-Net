using System;
using System.Diagnostics;
using System.IO;

namespace SwitchIP
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 一般输出
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// SQL
        /// </summary>
        SQL,
        /// <summary>
        /// Debug
        /// </summary>
        Debug
    }
    class WriteTxtLog
    {
        public string Dir { get; set; }
        public string Path { get; set; }
        WriteTxtLog log = null;
        readonly object @lock = new object();
        System.Threading.ReaderWriterLockSlim Slim = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);
        public WriteTxtLog Instance
        {
            get
            {
                if (log == null)
                {
                    if (log == null)
                    {
                        lock (@lock)
                        {
                            log = new WriteTxtLog();
                        }
                    }
                }
                return log;
            }
        }
        //private WriteTxtLog()
        //{

        //}
        /// <summary>
        /// 写入日志到指定文件名
        /// </summary>
        /// <param name="Msg">要写到日志文件的信息</param>
        public string WriteLineToFile(string Msg, LogType type, Type type_)
        {
            if (string.IsNullOrEmpty(Dir) || Dir == System.Windows.Forms.Application.StartupPath + @"\Logs\" + DateTime.Now.ToString("yyyyMMdd"))
            {
                return WriteLineToTimeFile(Msg, type, type_);
            }
            StreamWriter sw = null;
            try
            {
                Slim.EnterWriteLock();
                CheckLog(Dir);
                Checkfile(Dir, Path);
                LogInfo li = GetLog(type);
                string fileName = Dir + "\\" + Path;
                sw = File.AppendText(fileName);
                //sw.WriteLine("日志时间:" + DateTime.Now.ToString() + ",文件名:" + li.FileName + ",方法名：" + li.MethodName + "行号：" + li.Line + ",列：" + li.Column + ",日志类型:" + li.LogType);
                sw.WriteLine(DateTime.Now.ToString() + li.LogType + " " + type_.FullName + " " + Msg);
                return nameof(WriteLineToFile) + "日志记录操作成功！";
            }
            catch (Exception ex)
            {
                return nameof(WriteLineToFile) + "日志记录操作错误:" + ex.Message;
            }
            finally
            {
                sw.Close();
                Slim.ExitWriteLock();
            }
        }
        /// <summary>
        /// 在以小时为单位的文件名称里追加一行
        /// </summary>
        /// <param name="Msg">要写到日志文件的信息</param>
        string WriteLineToTimeFile(string Msg, LogType type, Type type_)
        {
            StreamWriter sw = null;
            try
            {
                Slim.EnterWriteLock();
                Dir = System.Windows.Forms.Application.StartupPath + @"\Logs\" + DateTime.Now.ToString("yyyyMMdd");
                CheckLog(Dir);
                string file = DateTime.Now.ToString("yyyyMMddHH") + ".log";
                Checkfile(Dir, file);
                string fileName = Dir + "\\" + file;
                LogInfo li = GetLog(type);
                sw = File.AppendText(fileName);
                //sw.WriteLine("日志时间:" + DateTime.Now.ToString() + ",文件名:" + li.FileName + ",方法名：" + li.MethodName + "行号：" + li.Line + ",列：" + li.Column + ",日志类型:" + li.LogType);
                sw.WriteLine(DateTime.Now.ToString() + li.LogType + " " + type_.FullName + " " + Msg);

                return nameof(WriteLineToTimeFile) + "日志记录操作成功！";
            }
            catch (Exception ex)
            {
                return nameof(WriteLineToTimeFile) + "日志记录发生错误:" + ex.Message;
            }
            finally
            {
                sw.Close();
                Slim.ExitWriteLock();
            }
        }
        //检查日志文件夹是否存在
        void CheckLog(string Path)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }
        /// <summary>
        /// 传入路径名称和文件名称，创建日志文件
        /// </summary>
        /// <param name="DirName"></param>
        /// <param name="FileName"></param>
        void Checkfile(string DirName, string FileName)
        {
            if (!File.Exists(DirName + @"\" + FileName))
            {
                File.Create(DirName + @"\" + FileName).Close();
            }
        }
        public string SetLogFilePath(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                return nameof(SetLogFilePath) + "输入参数不能为空！";
            }
            Dir = logFilePath.Substring(0, logFilePath.LastIndexOf("\\"));
            Path = logFilePath.Substring(logFilePath.LastIndexOf("\\") + 1, logFilePath.Length - logFilePath.LastIndexOf("\\") - 1);
            return nameof(SetLogFilePath) + "调用成功！";
        }
        internal struct LogInfo
        {
            internal string FileName { get; set; }
            internal string MethodName { get; set; }
            internal int Line { get; set; }
            internal int Column { get; set; }
            internal string LogType { get; set; }
        }
        internal LogInfo GetLog(LogType type)
        {
            StackTrace st = new StackTrace(2, true);
            StackFrame sf = st.GetFrame(0);
            LogInfo li = new LogInfo()
            {
                FileName = sf.GetFileName(),
                MethodName = sf.GetMethod().Name,
                Line = sf.GetFileLineNumber(),
                Column = sf.GetFileColumnNumber(),
            };
            string logType = "-Error";

            switch (type)
            {
                case LogType.Error:
                    logType = "-Error";
                    break;
                case LogType.Info:
                    logType = "-Info";
                    break;
                case LogType.Debug:
                    logType = "-Debug";
                    break;
                case LogType.Warning:
                    logType = "-Warning";
                    break;
                case LogType.SQL:
                    logType = "-SQL";
                    break;
                default:
                    logType = "-Error";
                    break;
            }
            li.LogType = logType;
            return li;
        }
    }
}
