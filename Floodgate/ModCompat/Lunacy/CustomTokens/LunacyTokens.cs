using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat.Lunacy.CustomTokens;

public static class LunacyTokens
{
    public readonly static List<IDetour> hooks = new List<IDetour>();
    internal static void Apply()
    {
        hooks.Add(new ILHook(typeof(global::Lunacy.CustomTokens.LunacyTokens).GetMethod("CollectiblesTracker_ctor", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic), IL_LunacyTokens_CollectiblesTracker_ctor));
        
    }
    public static void IL_LunacyTokens_CollectiblesTracker_ctor(ILContext context)
    {
        try
        {
            ILCursor c = new(context);
            if(c.TryGotoNext(MoveType.After, x=>x.MatchLdarg(2), x=>x.MatchLdfld<MainLoopProcess>("manager"),x=>x.MatchLdfld<ProcessManager>("rainWorld"), x => x.MatchStloc(0)))
            {
                ILLabel label = c.DefineLabel();
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate((RainWorld rainworld)=>
                {
                    return rainworld.progression.currentSaveState is null;
                });

                c.Emit(OpCodes.Brfalse_S, label);
                c.Emit(OpCodes.Ret);
                c.MarkLabel(label);
            }

        }catch(Exception e)
        {
            CustomLog.LogError("[MOD COMPAT] Lunacy IL_LunacyTokens_CollectiblesTracker_ctor fucked up\n" + e.ToString());
        }
    }
}
