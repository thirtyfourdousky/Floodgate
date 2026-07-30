using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using VoidTemplate.ModsCompatibilty;

namespace ModCompat;

public static class _LastWish
{
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(VoidTemplate.ModsCompatibilty._ModsMeta), "PostModsInit"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle handle);
        HookEndpointManager.Add(MethodBase.GetMethodFromHandle(handle), Handler);
    }

    public static void Handler(Action _)
    {
        foreach (ModManager.Mod mod in ModManager.ActiveMods)
        {
            try
            {
                switch (mod.id)
                {
                    case "blood":
                        Blood.Init();
                        break;
                    case "mosquitoes":
                        MosquitoCompat.Init();
                        break;
                    case "swalloweverything":
                        Floodgate.Plugin.logger.LogError("Please note that Swallow Everything is incompatible with Last Wish, floodgate just disables the error because reenabling the mods sucks");
                        break;
                    case "willowwisp.bellyplus":
                        RotundWorldApplyHolder();
                        break;
                }
            }catch (Exception ex)
            {
                CustomLog.LogError("Some of Last Wish's compat failed. Failed mod: " + ((mod?.id) ?? "null" + "\n" + ex));
            }
        }
    }

    //attributed by the InlineIL
#pragma warning disable CS0649
    static On.Player.hook_AddFood BPLWAddFood;
    static On.Player.hook_GrabUpdate BPLWGrabUpdate;
#pragma warning restore CS0649
    public static void RotundWorldApplyHolder()
    {
        try
        {
            RotundWorldApply();
        }
        catch (System.IO.FileNotFoundException) { }
        catch (Exception ex)
        {
            CustomLog.LogError("(external) Rotund World - Last Wish compat failed\n"+ex);
        }
    }
    public static void RotundWorldApply()
    {
        try
        {
            InlineIL.IL.Emit.Ldnull();
            InlineIL.IL.Emit.Ldftn(new(typeof(RotundWorld.BPLastWishFixes), "Player_AddFood"));
            InlineIL.IL.Emit.Newobj(new(typeof(On.Player.hook_AddFood), ".ctor"));
            InlineIL.IL.Emit.Stsfld(new(typeof(_LastWish), "BPLWAddFood"));

            InlineIL.IL.Emit.Ldnull();
            InlineIL.IL.Emit.Ldftn(new(typeof(RotundWorld.BPLastWishFixes), "Player_GrabUpdate"));
            InlineIL.IL.Emit.Newobj(new(typeof(On.Player.hook_GrabUpdate), ".ctor"));
            InlineIL.IL.Emit.Stsfld(new(typeof(_LastWish), "BPLWGrabUpdate"));

            On.Player.AddFood += BPLWAddFood;
            On.Player.GrabUpdate += BPLWGrabUpdate;
        }
        catch (Exception ex)
        {
            CustomLog.LogError("Rotund World - Last Wish compat failed\n" + ex);
        }
    }
}
