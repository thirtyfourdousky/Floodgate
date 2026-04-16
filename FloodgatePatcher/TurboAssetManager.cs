using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;

namespace FloodgatePatcher;

//i don't remember why it's called that
public static class TurboAssetManager
{
    public static readonly Dictionary<string, FileCache> PreloadFiles = new(System.StringComparer.OrdinalIgnoreCase);
    public static bool mappingFinished = false;

    internal static void Apply()
    {
        ModLoader.Hooks.Add(new Hook(typeof(System.IO.File).GetMethod("ReadAllLines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string) }, null), hook_ReadAllLines_path));
        ModLoader.Hooks.Add(new Hook(typeof(System.IO.File).GetMethod("ReadAllLines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(System.Text.Encoding) }, null), hook_ReadAllLines_path_encoding));
        ModLoader.Hooks.Add(new Hook(typeof(System.IO.File).GetMethod("ReadAllText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string) }, null), hook_ReadAllText_path));
        ModLoader.Hooks.Add(new Hook(typeof(System.IO.File).GetMethod("ReadAllText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(System.Text.Encoding) }, null), hook_ReadAllText_path_encoding));
    }
    public class FileCache(string AllText, string[] AllLines)
    {
        public readonly string AllText = AllText;
        public readonly string[] AllLines = AllLines;
        public readonly Dictionary<System.Text.Encoding, string> Enc_AllText = new();
        public readonly Dictionary<System.Text.Encoding, string[]> Enc_AllLines = new();
    }

    public delegate string[] orig_ReadAllLines_string(string path);
    public delegate string[] orig_ReadAllLines_string_Encoding(string path, System.Text.Encoding encoding);
    public delegate string orig_ReadAllText_string(string path);
    public delegate string orig_ReadAllText_string_Encoding(string path, System.Text.Encoding encoding);

    public static string[] hook_ReadAllLines_path(orig_ReadAllLines_string orig, string path)
    {
        if (mappingFinished && PreloadFiles.TryGetValue(path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), out FileCache cache))
        {
            return (string[])cache.AllLines.Clone();
        }
        return orig(path);
    }

    public static string[] hook_ReadAllLines_path_encoding(orig_ReadAllLines_string_Encoding orig, string path, System.Text.Encoding encoding)
    {
        if (mappingFinished && PreloadFiles.TryGetValue(path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), out FileCache cache))
        {
            if (cache.Enc_AllLines.TryGetValue(encoding, out string[] output))
            {
                return (string[])output.Clone();
            }
            return (string[])(cache.Enc_AllLines[encoding] = orig(path, encoding)).Clone();
        }
        return orig(path, encoding);
    }

    public static string hook_ReadAllText_path(orig_ReadAllText_string orig, string path)
    {
        if (mappingFinished && PreloadFiles.TryGetValue(path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), out FileCache cache))
        {
            return cache.AllText;
        }
        return orig(path);
    }

    public static string hook_ReadAllText_path_encoding(orig_ReadAllText_string_Encoding orig, string path, System.Text.Encoding encoding)
    {
        if (mappingFinished && PreloadFiles.TryGetValue(path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), out FileCache cache))
        {
            if(cache.Enc_AllText.TryGetValue(encoding, out string output))
            {
                return output;
            }
            return cache.Enc_AllText[encoding] = orig(path, encoding);
        }
        return orig(path, encoding);
    }
}
