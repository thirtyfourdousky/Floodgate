using FloodgatePatcher;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Floodgate._Modules.ExtendedEchoExtender;

public static class Hooks
{
    public static void Apply()
    {
        //On.GhostWorldPresence.GetGhostID += GhostWorldPresence_GetGhostID;
        //On.Ghost.ctor += Ghost_ctor;
        On.Room.Loaded += RoomOnLoaded;
        On.StoryGameSession.ctor += StoryGameSession_ctor;
        On.World.SpawnGhost += World_SpawnGhost;
        IL.ThreatDetermination.Update += IL_ThreatDetermination_Update;
        On.GhostWorldPresence.GhostMode_AbstractRoom_Vector2 += GhostWorldPresence_GhostMode_AbstractRoom_Vector2;
        IL.FliesWorldAI.AddFlyToSwarmRoom += IL_FliesWorldAI_AddFlyToSwarmRoom;
        IL.Room.NowViewed += IL_Room_NowViewed;
        IL.GoldFlakes.ctor += IL_GoldFlakes_ctor;
        IL.GoldFlakes.Update += IL_GoldFlakes_Update;
        IL.GhostCreatureSedater.Update += IL_GhostCreatureSedater_Update;
        On.RoomCamera.UpdateGhostMode += RoomCamera_UpdateGhostMode;
        On.InsectCoordinator.NowViewed += InsectCoordinator_NowViewed;
        IL.Music.PlayerThreatTracker.Update += IL_PlayerThreatTracker_Update;
    }

    /*private static void Ghost_ctor(On.Ghost.orig_ctor orig, Ghost self, Room room, PlacedObject placedObject, GhostWorldPresence worldGhost)
    {
        if(self is not EEEGhost)
        {
            orig(self, room, placedObject, worldGhost);
        }
    }*/

    private static void IL_PlayerThreatTracker_Update(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdloc(1) && x.Previous.MatchStloc(2) && x.Next.Next.MatchLdarg(0)))
            {
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)3);
                c.EmitDelegate(PlayerThreatTrackerUpdateLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_1);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook PlayerThreatTracker_Update couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook PlayerThreatTracker_Update fucked up\n" + ex.ToString());
        }
    }
    public static void PlayerThreatTrackerUpdateLogic(string _, Music.PlayerThreatTracker self, Player player, ref string text, ref float ghostmode)
    {
        if (!Registry.ExtraGhosts.TryGetValue(player.room.world, out var ghosts))
        { return; }

        foreach (var ghost in ghosts)
        {
            float _ghostmode = ghost.Value.GhostMode(player.room.abstractRoom, player.abstractCreature.world.RoomToWorldPos(player.mainBodyChunk.pos, player.room.abstractRoom.index));
            if(_ghostmode > ghostmode)
            {
                ghostmode = _ghostmode;
                text = ghost.Value.songName;
            }
        }

    }

    private static void InsectCoordinator_NowViewed(On.InsectCoordinator.orig_NowViewed orig, InsectCoordinator self)
    {
        if(Registry.ExtraGhosts.TryGetValue(self.room.world, out var ghosts))
        {
            foreach(var ghost in ghosts)
            {
                if (ghost.Value.CreaturesSleepInRoom(self.room.abstractRoom))
                {
                    return;
                }
            }
        }
        orig(self);
    }

    private static void RoomCamera_UpdateGhostMode(On.RoomCamera.orig_UpdateGhostMode orig, RoomCamera self, Room newRoom, int newCamPos)
    {
        orig(self,newRoom, newCamPos);
        if(self.spinningTopGhostMode || !Registry.ExtraGhosts.TryGetValue(newRoom.world, out var ghosts))
        {
            return;
        }
        bool changed = false;
        foreach (var ghost in ghosts)
        {
            if (ModManager.MMF)
            {
                ghost.Value.CleanSeperationDistance();
            }
            float newghostmode = ghost.Value.GhostMode(newRoom, newCamPos);
            if(newghostmode > self.ghostMode)
            {
                self.ghostMode = newghostmode;
                changed = true;
            }
        }
        if (changed)
        {
            self.lightBloomAlpha = self.ghostMode * 0.8f;
        }
    }

    private static void IL_GhostCreatureSedater_Update(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdloc(0) && x.Previous.MatchStloc(0) && x.Next.Next.Next.MatchCallvirt<UpdatableAndDeletable>("Destroy")))
            {
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)0);
                c.EmitDelegate(GhostCreatureSedaterUpdateLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook GhostCreatureSedater_Update couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook GhostCreatureSedater_Update fucked up\n" + ex.ToString());
        }
    }
    public static void GhostCreatureSedaterUpdateLogic(bool _, GhostCreatureSedater self, ref bool sleepInRoom)
    {
        if (sleepInRoom || !Registry.ExtraGhosts.TryGetValue(self.room.world, out var ghosts))
        {
            return;
        }
        foreach (var ghost in ghosts)
        {
            if (ghost.Value.CreaturesSleepInRoom(self.room.abstractRoom))
            {
                sleepInRoom = true;
                return;
            }
        }
    }

    private static void IL_GoldFlakes_Update(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdarg(0) && x.Next.MatchLdloc(0) && x.Next.Next.MatchCall<GoldFlakes>("NumberOfFlakes")))
            {
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)0);
                c.EmitDelegate(GoldFlakesUpdateLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook GoldFlakes_Update couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook GoldFlakes_Update fucked up\n" + ex.ToString());
        }
    }
    public static void GoldFlakesUpdateLogic(GoldFlakes self, ref float num)
    {
        if (!Registry.ExtraGhosts.TryGetValue(self.room.world, out var ghosts))
        {
            return;
        }
        foreach (var ghost in ghosts)
        {
            num = Mathf.Max(num, ghost.Value.GhostMode(self.room, self.savedCamPos));
        }
    }

    private static void IL_GoldFlakes_ctor(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdarg(0) && x.Next.MatchNewobj<List<GoldFlakes.GoldFlake>>()))
            {
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)0);
                c.EmitDelegate(GoldFlakesctorLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook GoldFlakes_ctor couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook GoldFlakes_ctor fucked up\n" + ex.ToString());
        }
    }
    public static void GoldFlakesctorLogic(GoldFlakes self, Room room, ref float num)
    {
        if(!Registry.ExtraGhosts.TryGetValue(room.world, out var ghosts))
        { 
            return;
        }
        foreach (var ghost in ghosts)
        {
            for (int i = 0; i < room.cameraPositions.Length; i++)
            {
                num = Mathf.Max(num, ghost.Value.GhostMode(room, i));
            }
        }
    }

    private static void IL_Room_NowViewed(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdarg(0) && x.Next.MatchLdfld<Room>("insectCoordinator") && x.Next.Next.MatchBrfalse(out _)))
            {
                c.EmitDelegate(RoomNowViewedLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook Room_NowViewed couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook Room_NowViewed fucked up\n" + ex.ToString());
        }
    }
    public static void RoomNowViewedLogic(Room self)
    {
        if(!Registry.ExtraGhosts.TryGetValue(self.world, out var ghosts))
        {
            return;
        }

        for (int i = 0; i < self.cameraPositions.Length; i++)
        {
            if (self.world.worldGhost is not null && self.world.worldGhost.GhostMode(self, i) > 0f)
            {
                return;
            }

            foreach (var ghost in ghosts)
            {
                if (ghost.Value.GhostMode(self, i) > 0f)
                {
                    self.AddObject(new GoldFlakes(self));
                    return;
                }
            }
        }
    }

    private static void IL_FliesWorldAI_AddFlyToSwarmRoom(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,x => x.MatchStloc(0)))
            {
                ILLabel label = c.DefineLabel();
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
                c.EmitDelegate(AddFlyToSwarmRoomLogic);
                c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse_S, label);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                c.MarkLabel(label);
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook FliesWorldAI AddFlyToSwarmRoom couldn't find injection point" + c.PrintInstrs());
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook FliesWorldAI AddFlyToSwarmRoom fucked up\n" + ex.ToString());
        }
    }
    public static bool AddFlyToSwarmRoomLogic(FliesWorldAI self, AbstractRoom swarmRoom)
    {
        if(!Registry.ExtraGhosts.TryGetValue(self.world, out var ghosts))
        {
            return false;
        }
        foreach(var ghost in ghosts)
        {
            if (ghost.Value.CreaturesSleepInRoom(swarmRoom))
            {
                return true;
            }
        }
        return false;
    }

    private static float GhostWorldPresence_GhostMode_AbstractRoom_Vector2(On.GhostWorldPresence.orig_GhostMode_AbstractRoom_Vector2 orig, GhostWorldPresence self, AbstractRoom testRoom, UnityEngine.Vector2 worldPos)
    {
        if(self is not EEEGhostWorldPresence)
        {
            return orig(self, testRoom, worldPos);
        }
        if(!Registry.echoesSettings.TryGetValue(self.ghostID, out var settings))
        {
            return 0f;
        }
        if(testRoom.index == self.ghostRoom.index)
        {
            return 1f;
        }
        float eff = settings.EffectRadius * 1000f;
        Vector2 vec = RWCustom.Custom.RestrictInRect(worldPos, FloatRect.MakeFromVector2(self.world.RoomToWorldPos(default(Vector2), self.ghostRoom.index), self.world.RoomToWorldPos(self.ghostRoom.size.ToVector2() * 20f, self.ghostRoom.index)));
        if (!RWCustom.Custom.DistLess(worldPos, vec, eff))
        {
            return 0f;
        }
        int separation = self.DegreesOfSeparation(testRoom);
        if(separation != -1)
        {
            return Mathf.Pow(Mathf.InverseLerp(eff, eff / 8f, Vector2.Distance(worldPos, vec)), 2f) * RWCustom.Custom.LerpMap(separation, 1f, 3f, 0.6f, 0.15f) * ((testRoom.layer != self.ghostRoom.layer) ? 0.6f : 1f);
        }
        return 0f;
    }

    private static void IL_ThreatDetermination_Update(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchStloc(2) && x.Next.MatchLdloc(2) && x.Next.Next.MatchLdcR4(0)))
            {
                IEnumerable<ILLabel> labels = c.IncomingLabels;
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (byte)2);
                c.EmitDelegate(ThreatDeterminationLogic);
                //c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_2);
                c.GotoPrev(MoveType.Before, x => x.MatchLdarg(0));
                foreach (ILLabel label in labels)
                {
                    label.Target = c.Next;
                }
            }
            else
            {
                CustomLog.LogError("[Extended Echo] ILHook ThreatDetermination Update could not find injection point" + c.PrintInstrs());
            }
        }catch (Exception ex)
        {
            CustomLog.LogError("[Extended Echo] ILHook ThreatDetermination Update fucked up\n" + ex.ToString());
        }
    }
    public static void ThreatDeterminationLogic(ThreatDetermination self, RainWorldGame game, Player player, ref float num4)
    {
        if (!player.room.world.HasExtraGhost())
        {
            return;
        }
        GhostWorldPresence ghostWorldPresence = null;
        foreach(var settings in Registry.echoesSettings)
        {
            if(string.Equals(settings.Value.Room, player.room.abstractRoom.name, StringComparison.OrdinalIgnoreCase))
            {
                ghostWorldPresence = player.room.world.ExtraGhost(settings.Key);
            }
        }
        if (ghostWorldPresence == null) { return; }
        if (game.cameras[0].ghostMode > (ModManager.MMF ? 0.1f : 0f))
        {
            float res = ghostWorldPresence.GhostMode(player.room.abstractRoom, player.abstractCreature.world.RoomToWorldPos(player.mainBodyChunk.pos, player.room.abstractRoom.index));
            num4 = res > num4 ? res : num4;
        }
    }

    private static void World_SpawnGhost(On.World.orig_SpawnGhost orig, global::World self)
    {
        orig(self);
        if (!(self.game.session is StoryGameSession session) || self.game.rainWorld.safariMode)
        {
            return;
        }
        foreach (var EchoID in Registry._extendedEchoesIDs)
        {
            if (Registry.echoesSettings.TryGetValue(EchoID, out var Echo))
            {
                if(!string.Equals(Echo.Region,self.name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                Dictionary<GhostWorldPresence.GhostID, GhostWorldPresence> extraghosts;
                if (!Registry.ExtraGhosts.TryGetValue(self, out extraghosts))
                {
                    Registry.ExtraGhosts.Add(self, extraghosts = new());
                }
                int ghostPreviouslyEncountered = 0;
                if (session.saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(EchoID))
                {
                    ghostPreviouslyEncountered = session.saveState.deathPersistentSaveData.ghostsTalkedTo[EchoID];
                }
                bool spawnghost = EEEGhostWorldPresence.SpawnGhost(EchoID, session.saveState.deathPersistentSaveData.karma, session.saveState.deathPersistentSaveData.karmaCap, ghostPreviouslyEncountered, self.game.StoryCharacter == SlugcatStats.Name.Red);

                if (ModManager.MSC && (!ModManager.Expedition || (ModManager.Expedition && !self.game.rainWorld.ExpeditionMode)))
                {
                    if (self.game.StoryCharacter == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer && !spawnghost)
                    {
                        if (ghostPreviouslyEncountered != 1)
                        {
                            spawnghost = false;
                        }
                        else if ((session.saveState.deathPersistentSaveData.karma == session.saveState.deathPersistentSaveData.karmaCap && session.saveState.deathPersistentSaveData.reinforcedKarma) || (ModManager.Expedition && self.game.rainWorld.ExpeditionMode && session.saveState.deathPersistentSaveData.karma == session.saveState.deathPersistentSaveData.karmaCap))
                        {
                            spawnghost = true;
                        }
                    }
                    if (self.game.StoryCharacter == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint)
                    {
                        spawnghost = false;
                        if (ghostPreviouslyEncountered == 2)
                        {
                            spawnghost = false;
                        }
                        else if (session.saveState.cycleNumber > 0)
                        {
                            spawnghost = true;
                        }
                    }
                }

                if (spawnghost && session.saveState.cycleNumber > 0)
                {
                    GhostWorldPresence extraghost = new EEEGhostWorldPresence(self, EchoID);
                    extraghosts.Add(EchoID, extraghost);
                    self.migrationInfluences.Add(extraghost);
                    CustomLog.Log("[Extended Echo] echo " + EchoID.value + " for region " + self.name);
                }
            }
        }
    }

    private static void StoryGameSession_ctor(On.StoryGameSession.orig_ctor orig, StoryGameSession self, SlugcatStats.Name saveStateNumber, RainWorldGame game)
    {
        Registry.LoadAllRegions(saveStateNumber);
        orig(self, saveStateNumber, game);
    }

    private static void RoomOnLoaded(global::On.Room.orig_Loaded orig, global::Room self)
    {
        if (self.game == null)
        {
            return;
        }
        orig(self);
        if (!self.world.HasExtraGhost())
        {
            return;
        }
        PlacedObject ghostObj = null;
        foreach (global::PlacedObject placedObject in self.roomSettings.placedObjects)
        {
            if (placedObject.type == Enums.EEEGhostSpot && placedObject.active)
            {
                ghostObj = placedObject;
                break;
            }
        }
        if (ghostObj is null)
        {
            return;
        }
        GhostWorldPresence.GhostID ghostID = null;
        //EchoSettings _settings;
        foreach (var settings in Registry.echoesSettings)
        {
            if (string.Equals(settings.Value.Room, self.abstractRoom.name, StringComparison.OrdinalIgnoreCase))
            {
                //_settings = settings.Value;
                ghostID = settings.Key;
                break;
            }
        }
        if (ghostID is null || ghostID == GhostWorldPresence.GhostID.NoGhost)
        {
            return;
        }
        GhostWorldPresence ghostWorldPresence = self.world.ExtraGhost(ghostID);

        if (ghostWorldPresence != null && ghostWorldPresence.ghostRoom == self.abstractRoom)
        {
            self.AddObject(new EEEGhost(self, ghostObj, ghostWorldPresence));
        }
        else if (self.world.region != null)
        {
            if (self.game.session is StoryGameSession && (!(self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(ghostID) || (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID] == 0))
            {
                self.AddObject(new GhostHunch(self, ghostID));
            }
        }
    }
}
