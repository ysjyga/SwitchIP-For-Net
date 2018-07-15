using System;

namespace SwitchIP
{
    public class LogHelper
    {
        //设置配置文件路径 程序路径 + 配置文件名
        string file = INIOperation.IniFilePath("SwitchIP.ini");
        static string IsWriteLog;
        static WriteTxtLog WriteLog = new WriteTxtLog();
        public static void IsWriteLog_(string str)
        {
            IsWriteLog = str;
        }
        public static void SQL(Type type, String message)
        {
            if (IsWriteLog == "Y")
            {
                WriteLog.WriteLineToFile(message, LogType.SQL, type);
            }
        }

        public static void Error(Type type, String message)
        {
            if (IsWriteLog == "Y")
            {
                WriteLog.WriteLineToFile(message, LogType.Error, type);
            }
        }

        public static void Warn(Type type, String message)
        {
            if (IsWriteLog == "Y")
            {
                WriteLog.WriteLineToFile(message, LogType.Warning, type);
            }
        }

        public static void Info(Type type, String message)
        {
            if (IsWriteLog == "Y")
            {
                WriteLog.WriteLineToFile(message, LogType.Info, type);
            }
        }

        public static void Debug(Type type, String message)
        {
            if (IsWriteLog == "Y")
            {
                WriteLog.WriteLineToFile(message, LogType.Debug, type);
            }
        }
        public void Main()
        {
            LogHelper.SQL(this.GetType(), "Fatal");
            LogHelper.Warn(this.GetType(), "Warn");
            LogHelper.Info(this.GetType(), "Warn");
            LogHelper.Debug(this.GetType(), "Debug");
            LogHelper.Error(this.GetType(), "Error");
        }
    }
}
