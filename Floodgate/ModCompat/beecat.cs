using BeeWorld;
using FloodgatePatcher;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;

namespace ModCompat;

public static class beecat
{
    internal static readonly List<IDetour> _hooks = new List<IDetour>();
    public static void Apply()
    {
        CustomLog.Log("Beecat apply..");
        _hooks.Add(new ILHook(typeof(BeeWorld.BupHook).GetMethod("GhostCreatureSedater_Update", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic), BupGhostCreatureSedaterUpdateOverride));
    }
    private static void BupGhostCreatureSedaterUpdateOverride(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            c.Goto(0);
            c.Instrs.Clear();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(delegate (ILContext iil)
            {
                ILCursor cursor = new ILCursor(iil);
                try
                {
                    int local = -1;
                    cursor.GotoNext(MoveType.After,
                        (Instruction x) => x.MatchCallvirt(out _),
                        (Instruction x) => x.MatchStloc(out local),
                        (Instruction x) => x.MatchLdloc(out _),
                        (Instruction x) => x.MatchLdfld("AbstractCreature","creatureTemplate") && x.Next.MatchLdfld("CreatureTemplate","ghostSedationImmune")
                        );
                    cursor.Remove();
                    cursor.EmitDelegate(delegate(CreatureTemplate template) { return (template.ghostSedationImmune || template.type == BeeEnums.CreatureType.Bup); });
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("beecat hook error: " + e.ToString() + "\n" + cursor.PrintInstrs());
                }
            });
            c.Emit(OpCodes.Ret);
        }
        catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError("beecat error: " + ex.ToString());
        }
    }
}
