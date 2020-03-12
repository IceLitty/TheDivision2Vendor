using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TheDivision2Vendor
{
    public class Logger
    {
        private static ConcurrentQueue<Log> logs = new ConcurrentQueue<Log>();

        public async static Task<Log> Take()
        {
            return await Task.Run(() =>
            {
                var a = new Log();
                while(!logs.TryDequeue(out a)) { Thread.SpinWait(1); }
                return a;
            });
        }

        public static void Put(LogPopType popup, LogType type, string msg)
        {
            Put(popup, type, msg, null);
        }

        public static void Put(LogPopType popup, LogType type, string msg, Exception e)
        {
            var l = new Log
            {
                dt = DateTime.Now,
                type = type,
                msg = msg,
                e = e,
                popup = popup
            };
            logs.Enqueue(l);
        }
    }

    public class Log
    {
        public DateTime dt;
        public string msg = String.Empty;
        public LogType type = LogType.Info;
        public Exception e;
        public LogPopType popup = LogPopType.Title;

        public override string ToString()
        {
            return e == null ? String.Format("{0} [{1}] {2}", dt.ToString("MM/dd/H:mm"), type.ToString(), msg) : String.Format("{0} [{1}] {2}\n{3}", dt.ToString("MM/dd/H:mm"), type.ToString(), msg, e.StackTrace);
        }
    }

    public enum LogType
    {
        Debug,
        Info,
        Warn,
    }

    public enum LogPopType
    {
        Title,
        File,
        Popup,
    }
}
