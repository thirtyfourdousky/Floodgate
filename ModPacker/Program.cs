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
        PathM.BulkCopy(Path.Combine(solutionPath, "FloodgateDownpour", "bin", "Debug", "Floodgate.dll"),
            "plugins",
            Path.Combine("v1.9.15b", "plugins"));
        PathM.BulkCopy(Path.Combine(solutionPath, "Modules", "FG-LBspecificDownpour", "bin", "Debug", "FGLBspecific.fdll"),
            Path.Combine("modules", "v1.9.15b"),
            Path.Combine("modules", "older"));
        
        //1.10 && 1.11
        PathM.BulkCopy(Path.Combine(solutionPath, "Floodgate", "bin", "Debug", "Floodgate.dll"),
            Path.Combine("newest", "plugins"),
            Path.Combine("v1.10.4", "plugins"));
        PathM.BulkCopy(Path.Combine(solutionPath, "Modules", "FG-LBspecific", "bin", "Debug", "FGLBspecific.fdll"),
            Path.Combine("modules", "v1.10.4"),
            Path.Combine("modules", "newest"));

        //patcher needs to be in ALL versioned paths
        PathM.BulkCopy(Path.Combine(solutionPath, "FloodgatePatcher", "bin", "Debug", "FloodgatePatcher.dll"),
            "patchers",
            Path.Combine("v1.9.15b", "patchers"),
            Path.Combine("v1.10.4", "patchers"),
            Path.Combine("newest", "patchers"));

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
                Console.WriteLine("Copying " + filepath + " to " + Path.Combine(Program.path, dest));
                FileInfo info = new(filepath);
                File.Copy(filepath, Path.Combine(Program.path, dest, info.Name), true);
                filepath = Path.Combine(info.Directory.FullName, info.Name.Split('.')[0] + ".pdb");
                info = new FileInfo(filepath);
                File.Copy(filepath, Path.Combine(Program.path, dest, info.Name), true);
                Console.WriteLine("Copied " + filepath + " to " + Path.Combine(Program.path, dest));
            }catch (Exception ex) { Console.WriteLine(ex.ToString()); Environment.Exit(0); }
        }
    }
}