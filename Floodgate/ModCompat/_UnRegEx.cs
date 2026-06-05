using Floodgate;
using FloodgatePatcher;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static partial class _UnRegEx
{
    internal static readonly List<IDetour> _hooks = new List<IDetour>();
    public static void Apply()
    {
        IL.Region.ctor_string_int_int_RainWorldGame_Timeline += UnRegEx.unregex;
        if (FGTools.IsModActive("crs"))
        {
            try
            {
                Apply_CRS();
            }
            catch (Exception ex)
            {
                CustomLog.LogError("[UnRegEx] CRS apply failed miserably\n" + ex.ToString());
            }
        }
    }
    public static void Apply_CRS()
    {
        try
        {
            _hooks.Add(new ILHook(typeof(CustomRegions.RegionProperties.RegionProperties).GetMethod("GenerateProperties", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic), UnRegEx.unregex));
            _hooks.Add(new ILHook(typeof(CustomRegions.CustomWorld.WorldLoaderHook).GetMethod("FromConnectionsToList", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic), UnRegEx.unregex));
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[UnRegEx] CRS hooking failed miserably\n" + ex.ToString());
        }
    }
}
