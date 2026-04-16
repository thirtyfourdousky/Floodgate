using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace FloodgatePatcher
{
    public static class CustomLog
    {
        static CustomLog()
        {
            System.AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            writerThread.Start();
        }

        private static void CurrentDomain_ProcessExit(object sender, System.EventArgs e)
        {
            Close();
        }

        public static Thread writerThread = new(() => {
            StreamWriter writer = new StreamWriter(ModLoader.LogPath, true);
            while (threadrunning)
            {
                signal.WaitOne();
                while(queue.TryDequeue(out object msg))
                {
                    writer.WriteLine(msg);
                }
                writer.Flush();
            }
            writer.Close();
        }) { IsBackground = true };

        public static bool threadrunning = true;
        public static readonly ConcurrentQueue<object> queue = new();
        public static readonly AutoResetEvent signal = new(false);
        internal static bool active = false;
        public static void Log(string message)
        {
            if (!active || !threadrunning)
            {
                Patcher.logger?.LogDebug(message);
                return;
            }
            try
            {
                Write(message);
            }
            catch(System.Exception ex)
            {
                Patcher.logger?.LogError(ex);
                Patcher.logger?.LogDebug(message);
                return;
            }

        }
        public static void LogError(string message)
        {
            if (!active || !threadrunning)
            {
                Patcher.logger?.LogError(message);
                return;
            }
            try
            {
                Write("[ERROR] " + message);
            }
            catch (System.Exception ex)
            {
                Patcher.logger?.LogError(ex);
                Patcher.logger?.LogError(message);
                return;
            }

        }
        public static void Write(object msg = null)
        {
            queue.Enqueue(msg?.ToString());
            signal.Set();
        }
        public static void Close() //in theory i should never need to close this...
        {
            threadrunning = false;
            signal.Set();
        }
    }
}
