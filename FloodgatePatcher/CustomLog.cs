using System.IO;

namespace FloodgatePatcher
{
    public static class CustomLog
    {
        private static readonly object logLock = new object();
        internal static bool active = false;
        public static void Log(string message)
        {
            lock (logLock)
            {
                if (!active)
                {
                    Patcher.logger?.LogDebug(message);
                    return;
                }
                try
                {
                    using (StreamWriter writer = new StreamWriter(ModLoader.LogPath, true))
                    {
                        writer.WriteLine(message);
                    }
                }
                catch
                {
                    Patcher.logger?.LogDebug(message);
                    return;
                }
            }
        }
        public static void LogError(string message)
        {
            lock (logLock)
            {
                if (!active)
                {
                    Patcher.logger?.LogError(message);
                    return;
                }
                try
                {
                    using (StreamWriter writer = new StreamWriter(ModLoader.LogPath, true))
                    {
                        writer.WriteLine("[[ERROR]]" + message);
                    }
                }
                catch
                {
                    Patcher.logger?.LogError(message);
                    return;
                }
            }
        }
    }
}
