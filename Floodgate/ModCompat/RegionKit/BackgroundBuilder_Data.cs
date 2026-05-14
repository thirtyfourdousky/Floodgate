using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModCompat.RegionKit;

public static class BackgroundBuilder_Data
{
    public readonly static List<IDetour> hooks = new List<IDetour>();
    public static void Apply()
    {
        hooks.Add(new ILHook(typeof(global::RegionKit.Modules.BackgroundBuilder.Data).GetMethod("RoomSettings_Load", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic), IL_RoomSettings_Load));
    }
    public static void IL_RoomSettings_Load(ILContext context)
    {
        try
        {
            ILCursor c = new ILCursor(context);
            System.Reflection.MethodInfo RESplit = typeof(System.Text.RegularExpressions.Regex).GetMethod("Split", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(string) }, null);
            //List<Instruction> targets = c.Instrs.Where(i => i.MatchCall<System.Text.RegularExpressions.Regex>("Split")).ToList();
            List<Instruction> targets = c.Instrs.Where(i => i.MatchCall(RESplit)).ToList();
            if (targets.Count == 0)
            {
                CustomLog.Log("[Mod Compat] RegionKit BackgroundBuilder.Data RoomSettings_Load failed");
            }
            int count = 0;
            for(int i = 0; i < targets.Count; i++)
            {
                c.Goto(targets[i]);
                IEnumerable<ILLabel> labels = c.IncomingLabels;
                c.Remove();
                c.EmitDelegate(REsplitToStringSplit);
                foreach (ILLabel label in labels)
                {
                    label.Target = c.Prev;
                }
                count++;
            }
            CustomLog.Log("[Mod Compat] RegionKit BackgroundBuilder.Data RoomSettings_Load replaced " + count + " instructions");
        }catch(Exception ex)
        {
            CustomLog.LogError("[Mod Compat] RegionKit BackgroundBuilder.Data RoomSettings_Load fucked up\n"+ ex.ToString());
        }
    }

    public static string[] REsplitToStringSplit(string _string, string pattern)
    {
        return _string.Split(new string[] { pattern }, StringSplitOptions.None);
    }
}
