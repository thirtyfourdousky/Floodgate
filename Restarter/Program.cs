// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Diagnostics;

public static class Program
{
    static void Main(string[] args)
    {
        try
        {
            start(args);
        }catch (Exception ex)
        {
            Console.WriteLine("floodgate auto restarter launcher fucked up, please report this");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }

    static void start(string[] args)
    {
        int oldPID = int.Parse(Environment.GetEnvironmentVariable("RWOLDPID") ?? "-1");
        string rwPath = Environment.GetEnvironmentVariable("RWGAMEPAH") ?? "";

        if (oldPID == -1 || string.IsNullOrWhiteSpace(rwPath))
        {
            Console.WriteLine("floodgate auto restarter launcher fucked up, please report this");
            Console.WriteLine("PID or PATH null");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            return;
        }

        try
        {
            var proc = Process.GetProcessById(oldPID);
            proc.WaitForExit();
        }
        catch { }

        Thread.Sleep(1000); //arbitrary

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.EnvironmentVariables.Clear();
        //assuming i did it right, autorestarter already passed the right variables and args
        foreach (DictionaryEntry i in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process))
        {
            psi.EnvironmentVariables[(string)i.Key] = (string)i.Value!;
        }
        psi.UseShellExecute = false;
        psi.FileName = rwPath;
        psi.Arguments = string.Join(" ", args);
        Process.Start(psi);
    }
}