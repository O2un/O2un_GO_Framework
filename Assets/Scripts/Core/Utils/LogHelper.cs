using System;
using UnityEngine;

namespace O2un.Core.Utils
{
    public class LogManager
    {
        static private LogManager _instance;
        private LogManager() {}
        public static LogManager Instance => _instance ??= new();
        
        public void LogToEditor(LogHelper.LogLevel type, string str)
        {
            switch (type)
            {
                case LogHelper.LogLevel.Trace:
                case LogHelper.LogLevel.Debug:
                case LogHelper.LogLevel.Info:
                    {
                        Debug.Log(str);
                    }
                    break;
                case LogHelper.LogLevel.Warning: Debug.LogWarning(str); break;
                case LogHelper.LogLevel.Error: 
                case LogHelper.LogLevel.Fatal:
                    {
                        Debug.LogError(str);
                    }
                    break;
            }
        }

        internal void LogToFile(LogHelper.LogLevel type, string str)
        {
            // NULL
        }
    }

    public static class LogHelper
    {
        public enum LogLevel
        {
            Trace, // 디버깅
            Debug, // 디버깅
            Info,  // 일반 정보 (실행 기기정보 등)
            Warning, // 잠재적으로 문제가 발생 할 수 있는 경고
            Error, // 반드시 처리해야하는 에러
            Fatal, // 치명적인 오류 (프로그램 종료해야 하는 수준)
        }
    
        public enum LogFilter
        {
            None,
    
            Server,
            Client,
            Etc,
        }

        public static void Dev(string str, LogLevel type = LogLevel.Debug)
        {
            #if UNITY_EDITOR
            LogManager.Instance.LogToEditor(type, str);
            #endif
        }
    
        public static void Log(LogLevel type, string str, LogFilter filter = LogFilter.None)
        {
            string filtered = (LogFilter.None == filter) ? str :  $"{filter.ToStringOpt()} - {str}";
            
            LogToEditor(type, filtered);
            LogToFile(type, filtered);
            Console.WriteLine(filtered);
        }
    
        [System.Diagnostics.Conditional("ENABLE_EDITOR_LOG")]
        private static void LogToEditor(LogLevel type, string str)
        {
            LogManager.Instance.LogToEditor(type, str);
        }
        

        [System.Diagnostics.Conditional("ENABLE_FILE_LOG")]
        private static void LogToFile(LogLevel type, string str)
        {
            LogManager.Instance.LogToFile(type, str);
        }
    }
}
