using FloodgatePatcher;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class RemixAutoRestarter
{
    internal static readonly List<IDetour> _hooks = new List<IDetour>();
    public static void Apply_MMF()
    {
        _hooks.Add(new ILHook(typeof(MenuFixes.Mods.RemixAutoRestart).GetMethod("Restart", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static), IL_Restart_MMF));
    }

    public static void IL_Restart_MMF(ILContext IL)
    {
        try
        {
            var startMethodInfo = typeof(System.Diagnostics.Process).GetMethod("Start", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(System.Diagnostics.ProcessStartInfo) }, null);
            ILCursor cursor = new(IL);
            //this is defenitely breaking on a update
            cursor.GotoNext(MoveType.After, x=>x.MatchLdloc(7) && x.Next.MatchCall(startMethodInfo));
            cursor.Remove();
            cursor.EmitDelegate(EditRestart);

        }
        catch (Exception ex)
        {
            CustomLog.LogError("[MOD COMPAT] Many Menu Fixes autorestarter hook failed\n" + ex);
        }
    }



    public static System.Diagnostics.Process EditRestart(System.Diagnostics.ProcessStartInfo info)
    {
        info.EnvironmentVariables.Add("RWGAMEPAH", info.FileName);
        info.EnvironmentVariables.Add("RWOLDPID", System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
        info.FileName = ModLoader.FloodgatePath + Path.DirectorySeparatorChar + "reload" + Path.DirectorySeparatorChar + "Restarter.exe";
        return System.Diagnostics.Process.Start(info);
    }
}
