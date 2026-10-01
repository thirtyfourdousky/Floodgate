using FloodgatePatcher;
using LBMergedMods.Creatures;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WetandHot;

namespace ModCompat.WetAndHot;

public static class M4rblelous
{
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("WetandHot", "WetandHot.Plugin"), "PhysicalObject_ctor"));
        InlineIL.IL.Pop(out RuntimeMethodHandle POctor);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(POctor), (ILContext.Manipulator)IL_PhysicalObject_ctor);


        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(WetandHot.WetandHotModuleManager), "GetWaHData"));
        InlineIL.IL.Pop(out RuntimeMethodHandle GetWaHData);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(GetWaHData), (ILContext.Manipulator)IL_GetWaHData);

        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(FatFireFly), "Collide")); InlineIL.IL.Pop(out RuntimeMethodHandle VultureCollide);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(VultureCollide), (ILContext.Manipulator)IL_FatFireFlyCollision);

        Main.LizardSpits[typeof(LizardWaterSpit)] = .1f;
    }

    public static void IL_PhysicalObject_ctor(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After, x => x.MatchNewobj<PhysicalObjectModule>());
            c.Emit(OpCodes.Dup);
            c.Emit(OpCodes.Ldarg_2);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(M4rblelous), "SetupWaHLB"));
            InlineIL.IL.Pop(out RuntimeMethodHandle handle);
            c.Emit(OpCodes.Call, System.Reflection.MethodBase.GetMethodFromHandle(handle));
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }

    public static void IL_GetWaHData(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After, x => x.MatchNewobj<PhysicalObjectModule>());
            c.Emit(OpCodes.Dup);
            c.Emit(OpCodes.Ldarg_0);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(M4rblelous), "SetupWaHLB"));
            InlineIL.IL.Pop(out RuntimeMethodHandle handle);
            c.Emit(OpCodes.Call, System.Reflection.MethodBase.GetMethodFromHandle(handle));
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }

    public static void SetupWaHLB(PhysicalObjectModule data, PhysicalObject obj)
    {
        if(obj is FatFireFly)
        {
            data.Heat = 5f;
            data.StandardHeat = 5f;
            data.MaxHeat = 25f;
            data.HeatDamageTolerance = 0f;
            data.HeatTolerance = float.PositiveInfinity;
            data.HeatingRate = 5f;
        }
        else if(obj is TintedBeetle)
        {
            data.Heat = 2f;
            data.StandardHeat = 2f;
            data.MaxHeat = 5f;
            data.HeatDamageTolerance = 0f;
            data.HeatTolerance = float.PositiveInfinity;
            data.HeatingRate = 2f;
        }
        else if(obj is WaterBlob)
        {
            data.Damp = 2f;
            data.StandardDamp = 2f;
            data.MaxDamp = 5f;
            data.DampDamageTolerance = 0f;
            data.DampTolerance = float.PositiveInfinity;
            data.DampingRate = 2f;
        }
    }

    public static void IL_FatFireFlyCollision(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            c.GotoNext(x => x.MatchLdcI4(out _) && x.Previous.MatchLdloc(0), x => x.MatchCallvirt<Creature>("Stun"));
            c.Emit(OpCodes.Dup);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(M4rblelous), "FatFireFlyStun")); InlineIL.IL.Pop(out RuntimeMethodHandle handle);
            c.Emit(OpCodes.Call, System.Reflection.MethodBase.GetMethodFromHandle(handle));
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
    
    public static void FatFireFlyStun(Creature crit)
    {
        if(crit is not TintedBeetle)
        {
            crit.GetWaHData().Heat += 0.25f;
        }
    }
}
