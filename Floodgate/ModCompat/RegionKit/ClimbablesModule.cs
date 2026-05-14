using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;

namespace ModCompat.RegionKit;

public static class ClimbablesModule
{
    public static readonly List<IDetour> _hooks = new();
    public static void Apply()
    {
        _hooks.Add(new ILHook(typeof(global::RegionKit.Modules.Climbables._Module).GetMethod("ClimbableVinesSystem_VineSwitch_hk", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic), IL_ClimbableVinesSystem_VineSwitch_hk));
    }

    public static void IL_ClimbableVinesSystem_VineSwitch_hk(ILContext il)
    {
        try
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,x => x.MatchCallvirt<IClimbableVine>("TotalPositions"));
            if (!c.Next.MatchLdcI4(1))
            {
                c.Emit(OpCodes.Ldc_I4_1);
                c.Emit(OpCodes.Sub);
            }
            else
            {
                CustomLog.Log("[Mod Compat] RegionKit ClimbableVinesSystem_VineSwitch_hk IL hook is probably outdated, you should report this");
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Mod Compat] RegionKit ClimbableVinesSystem_VineSwitch_hk IL hook fucked up\n" + ex.ToString());
        }
    }
}
