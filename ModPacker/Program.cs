using System;
using System.IO;

namespace ModPacker;

public static class Program
{
    public static string path = Directory.GetCurrentDirectory();
    public static string solutionPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;
    static void Main(string[] args)
    {
        Console.WriteLine(string.Join(" ", args));
        Console.WriteLine("Current path " + path);
        Console.WriteLine("Solution path " + solutionPath);

        Console.WriteLine("Is that right? {Y/N}");
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("Proceeding");
            }
            else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Exitting...");
                Environment.Exit(0);
            }
        } while (key.Key != ConsoleKey.Y && key.Key != ConsoleKey.N && key.Key != ConsoleKey.Escape);

        //1.9.15b
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgateDownpour" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "Floodgate.dll"),
            "plugins",
            ("v1.9.15b" + Path.DirectorySeparatorChar + "plugins"));
        
        //1.10 && 1.11
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "Floodgate" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "Floodgate.dll"),
            ("newest" + Path.DirectorySeparatorChar + "plugins"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "plugins"));
        //Extra
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgateExtra" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "FloodgateExtra.dll"),
            ("newest" + Path.DirectorySeparatorChar + "plugins"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "plugins"));

        //patcher needs to be in ALL versioned paths
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgatePatcher" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "FloodgatePatcher.dll"),
            "patchers",
            ("v1.9.15b" + Path.DirectorySeparatorChar + "patchers"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "patchers"),
            ("newest" + Path.DirectorySeparatorChar + "patchers"));

        //pdb
        //1.9.15b
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgateDownpour" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "Floodgate.pdb"),
            "plugins",
            ("v1.9.15b" + Path.DirectorySeparatorChar + "plugins"));
        
        //1.10 && 1.11
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "Floodgate" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "Floodgate.pdb"),
            ("newest" + Path.DirectorySeparatorChar + "plugins"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "plugins"));
        //Extra
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgateExtra" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "FloodgateExtra.pdb"),
            ("newest" + Path.DirectorySeparatorChar + "plugins"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "plugins"));

        //patcher needs to be in ALL versioned paths
        PathM.BulkCopy((solutionPath + Path.DirectorySeparatorChar + "FloodgatePatcher" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug" + Path.DirectorySeparatorChar + "FloodgatePatcher.pdb"),
            "patchers",
            ("v1.9.15b" + Path.DirectorySeparatorChar + "patchers"),
            ("v1.10.4" + Path.DirectorySeparatorChar + "patchers"),
            ("newest" + Path.DirectorySeparatorChar + "patchers"));

    }
}

public static class PathM
{
    public static void BulkCopy(string filepath, params string[] destinations)
    {
        foreach (string dest in destinations)
        {
            try
            {
                Console.WriteLine("Copying " + filepath + " to " + (Program.path + Path.DirectorySeparatorChar + dest));
                FileInfo info = new(filepath);
                File.Copy(filepath, (Program.path + Path.DirectorySeparatorChar + dest + Path.DirectorySeparatorChar + info.Name), true);
                Console.WriteLine("Copied " + filepath + " to " + (Program.path + Path.DirectorySeparatorChar + dest));
            }catch (Exception ex) { Console.WriteLine(ex.ToString()); Environment.Exit(0); }
        }
    }
}