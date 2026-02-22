using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace ModCompat;
public static class NotScugPlaySpupSafari
{
    public static readonly List<IDetour> hooks = new List<IDetour>();
    public static void Apply()
    {
        hooks.Add(new ILHook(typeof(FieldTrip.FieldTripMain).GetMethod("playerGraphicsInitSpritesHook", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), IL_playerGraphicsInitSpritesHook));
        hooks.Add(new ILHook(typeof(SprobDesecratingGraves.PlayerHooks).GetMethod("GraphicsInitiateSprites", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static), IL_GraphicsInitiateSprites));
        CustomLog.Log("[NotScugPlay+SpupSafari Compat] Found Not Slugcat Playables and Slugpup Safari, applied compatibility hooks");
    }
    public static void IL_playerGraphicsInitSpritesHook(ILContext IL)
    {
        try
        {
            ILCursor c = new(IL);
            if (c.Instrs[10].MatchLdarg(3) && c.Instrs[13].OpCode == OpCodes.Stfld)
            {
                c.Goto(11);
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_2);
                c.EmitDelegate((RoomCamera.SpriteLeaser sLeaser, PlayerGraphics playerGraphics) =>
                {
                    if (SprobDesecratingGraves.Main.isRotPlayer(playerGraphics.player))
                    {
                        sLeaser.containers = new FContainer[2]
                        {
                            new FContainer(),
                            new FContainer()
                        };
                    }
                    else
                    {
                        sLeaser.containers = new FContainer[0];
                    }
                });
            }
            else
            {
                CustomLog.LogError("[NotScugPlay+SpupSafari Compat] Could not find injection point (IL_playerGraphicsInitSpritesHook)\n" + IL.ToString());
            }
        }catch(System.Exception e)
        {
            CustomLog.LogError("[NotScugPlay+SpupSafari Compat] IL hook 'playerGraphicsInitSpritesHook' failed\n" + e.ToString());
        }
    }
    public static void IL_GraphicsInitiateSprites(ILContext IL)
    {
        try
        {
            ILCursor c = new(IL);
            if (c.Instrs[14].MatchLdarg(2) && c.Instrs[25].OpCode == OpCodes.Stfld)
            {
                c.Goto(14);
                c.RemoveRange(12);
            }
            else
            {
                CustomLog.LogError("[NotScugPlay+SpupSafari Compat] Could not find injection point (IL_GraphicsInitiateSprites)\n" + IL.ToString());
            }
        }
        catch (System.Exception e)
        {
            CustomLog.LogError("[NotScugPlay+SpupSafari Compat] IL hook 'GraphicsInitiateSprites' failed\n" + e.ToString());
        }
    }
}
