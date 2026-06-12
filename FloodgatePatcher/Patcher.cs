using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FloodgatePatcher;

public static class Patcher
{
    public static ManualLogSource? _logger = null;
    public static ManualLogSource logger
    {
        get
        {
            if( _logger == null)
            {
                _logger = Logger.CreateLogSource("FloodgatePatcher");
            }
            return _logger;
        }
        private set
        {
            _logger = value;
        }
    }
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    public static void Patch(ref AssemblyDefinition assembly)
    {
    }

    public static void Initialize()
    {
        logger = Logger.CreateLogSource("FloodgatePatcher");
        ModLoader.Init();
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
    }

    private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        try
        {
            ModLoader.LoadedAssemblies.Add(args.LoadedAssembly.FullName);
        }
        catch (Exception e)
        {
            CustomLog.LogError(e.ToString());
        }
        try
        {
            if (ModLoader.debug)
            {
                CustomLog.Log("[ASSEMBLY LOAD DEBUG] Assembly: " + args.LoadedAssembly.FullName + "\n Path: " + args.LoadedAssembly.Location ?? "...how is the path null?");
            }
        }catch(Exception ex)
        {
            CustomLog.LogError("[ASSEMBLY LOAD DEBUG] threw an error: " + ex.ToString());
        }
    }

    public static void Finish()
    {
        CustomLog.Log("Running post Patchers");
        ModLoader.Hooks.Add(new Hook(typeof(PluginInfo).GetProperty("Location", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), ModLoader.On_PluginInfo_Location));
        ModLoader.AssemblyCSharp = AppDomain.CurrentDomain.GetAssemblies().First(i => i.GetName().Name == "Assembly-CSharp");

        OtherHooks.Apply();
        //TurboAssetManager.Apply();
    }
}
