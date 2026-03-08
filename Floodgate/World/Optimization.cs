using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate.World;

public static class Optimization
{
    public static volatile WorldLoader.LoadingContext lastLoadingContext = null;
    public static void Apply()
    {
        IL.Region.ctor_string_int_int_RainWorldGame_Timeline += Region_ctor_string_int_int_RainWorldGame_Timeline;
        IL.WorldLoader.NextActivity += WorldLoader_NextActivity;
        IL.WorldLoader.CreatingAbstractRooms += WorldLoader_CreatingAbstractRooms;
        IL.Watcher.WarpMap.TryGoToRegionMap += WarpMap_TryGoToRegionMap;
        On.WorldLoader.CreatingWorld += WorldLoader_CreatingWorld;
        On.WorldLoader.ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues += WorldLoader_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues;
        On.WorldLoader.ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues_LoadingContext += WorldLoader_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues_LoadingContext;
        //On.Region.ReloadRoomSettingsTemplate += Region_ReloadRoomSettingsTemplate;
    }

    private static void Region_ctor_string_int_int_RainWorldGame_Timeline(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdloc(10) && x.Next.MatchLdstr("Room Setting Templates")))
            {
                c.RemoveRange(2);
                c.EmitDelegate((string load) => { return lastLoadingContext != WARPMAPLOADING && load == "Room Setting Templates"; });
            }
            else
            {
                CustomLog.LogError("[Warp Map optimization] IL Region Ctor couldn't find injection point" + delegate () { c.Goto(0); return c.ToString(); });
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Warp Map optimization] IL Region Ctor fucking failed\n" + ex.ToString());
        }
    }

    private static void WorldLoader_NextActivity(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before, x => x.MatchLdsfld<global::WorldLoader.LoadingContext>("FASTTRAVEL")))
            {
                object label = c.Next.Next.Next.Operand;
                object extenumEq = c.Next.Next.Operand;
                object loadcontext = c.Prev.Operand;
                c.EmitDelegate(() => { return WARPMAPLOADING; });
                c.Emit(OpCodes.Call, extenumEq);
                c.Emit(OpCodes.Brtrue_S, label);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, loadcontext);
            }
            else
            {
                CustomLog.LogError("[Warp Map optimization] IL NextActivity couldn't find injection point" + delegate () { c.Goto(0); return c.ToString(); });
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Warp Map optimization] IL NextActivity fucking failed\n" + ex.ToString());
        }
    }

    private static void WorldLoader_CreatingAbstractRooms(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before, x => x.MatchLdsfld<global::WorldLoader.LoadingContext>("FASTTRAVEL")))
            {
                object label = c.Next.Next.Next.Operand;
                object extenumIneq = c.Next.Next.Operand;
                object loadcontext = c.Prev.Operand;
                c.EmitDelegate(() => { return WARPMAPLOADING; });
                c.Emit(OpCodes.Call, extenumIneq);
                c.Emit(OpCodes.Brfalse_S, label);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, loadcontext);
            }
            else
            {
                CustomLog.LogError("[Warp Map optimization] IL CreatingAbstractRooms couldn't find injection point" + delegate () { c.Goto(0); return c.ToString(); });
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Warp Map optimization] IL CreatingAbstractRooms fucking failed\n" + ex.ToString());
        }
    }

    private static void WarpMap_TryGoToRegionMap(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if(c.TryGotoNext(MoveType.Before,x=>x.MatchLdsfld<global::WorldLoader.LoadingContext>("FASTTRAVEL")))
            {
                c.Remove();
                c.EmitDelegate(() => { return WARPMAPLOADING; });
            }
            else
            {
                CustomLog.LogError("[Warp Map optimization] IL TryGoToRegionMap couldn't find injection point" + delegate() { c.Goto(0); return c.ToString(); });
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[Warp Map optimization] IL TryGoToRegionMap fucking failed\n" + ex.ToString());
        }
    }

    private static void WorldLoader_CreatingWorld(On.WorldLoader.orig_CreatingWorld orig, WorldLoader self)
    {
        if (self.loadContext != WARPMAPLOADING)
        {
            orig(self);
            return;
        }
        /*self.world.spawners = self.spawners.ToArray();
        List<global::World.Lineage> list = new List<global::World.Lineage>();
        for (int i = 0; i < self.spawners.Count; i++)
        {
            if (self.spawners[i] is global::World.Lineage)
            {
                list.Add((self.spawners[i] as global::World.Lineage)!);
            }
        }*/
        //self.world.lineages = list.ToArray();
        self.world.lineages = Array.Empty<global::World.Lineage>();
        self.world.LoadWorldForFastTravel(self.timelinePosition, self.abstractRooms, self.swarmRoomsList.ToArray(), self.sheltersList.ToArray(), self.gatesList.ToArray());
        self.fliesMigrationBlockages = new int[0,2];
        /*self.fliesMigrationBlockages = new int[self.tempBatBlocks.Count, 2];
        for (int j = 0; j < self.tempBatBlocks.Count; j++)
        {
            int num = ((self.world.GetAbstractRoom(self.tempBatBlocks[j].fromRoom) == null) ? (-1) : self.world.GetAbstractRoom(self.tempBatBlocks[j].fromRoom).index);
            int num2 = ((self.world.GetAbstractRoom(self.tempBatBlocks[j].destRoom) == null) ? (-1) : self.world.GetAbstractRoom(self.tempBatBlocks[j].destRoom).index);
            self.fliesMigrationBlockages[j, 0] = num;
            self.fliesMigrationBlockages[j, 1] = num2;
        }*/
        /*if (ModManager.MSC && self.game != null && self.game.wasAnArtificerDream)
        {
            return;
        }
        if (self.game != null && self.setupValues.worldCreaturesSpawn && self.game.session is StoryGameSession && !self.world.singleRoomWorld)
        {
            self.GeneratePopulation((self.game.session as StoryGameSession)!.saveState.regionLoadStrings[self.world.region.regionNumber] == null);
        }
        if (self.game != null && self.game.session is StoryGameSession && !self.world.singleRoomWorld && self.world.region != null && (self.world.region.name == "SL" || self.world.region.name == "RM") && SlugcatStats.AtOrAfterTimeline(self.timelinePosition, SlugcatStats.Timeline.Rivulet) && self.game.IsMoonActive())
        {
            int num3 = global::UnityEngine.Random.Range(3, 8);
            if (self.world.region.name == "RM")
            {
                num3 = global::UnityEngine.Random.Range(2, 4);
            }
            for (int k = 0; k < num3; k++)
            {
                AbstractCreature abstractCreature = new AbstractCreature(self.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Overseer), null, new WorldCoordinate(self.world.offScreenDen.index, -1, -1, 0), self.game.GetNewID());
                abstractCreature.creatureTemplate.saveCreature = false;
                (abstractCreature.abstractAI as OverseerAbstractAI)!.moonHelper = true;
                (abstractCreature.abstractAI as OverseerAbstractAI)!.ownerIterator = 1;
                self.world.offScreenDen.entitiesInDens.Add(abstractCreature);
            }
        }*/
    }

    private static void WorldLoader_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues_LoadingContext(On.WorldLoader.orig_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues_LoadingContext orig, WorldLoader self, RainWorldGame game, SlugcatStats.Name playerCharacter, SlugcatStats.Timeline timelinePosition, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues, WorldLoader.LoadingContext context)
    {
        orig(self, game, playerCharacter, timelinePosition, singleRoomWorld, worldName, region, setupValues, context);
        lastLoadingContext = context;
    }

    private static void Region_ReloadRoomSettingsTemplate(On.Region.orig_ReloadRoomSettingsTemplate orig, Region self, string templateName)
    {
        if (lastLoadingContext == WARPMAPLOADING)
        {

            return;
        }
        orig(self,templateName);
    }

    private static void WorldLoader_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues(On.WorldLoader.orig_ctor_RainWorldGame_Name_Timeline_bool_string_Region_SetupValues orig, WorldLoader self, RainWorldGame game, SlugcatStats.Name playerCharacter, SlugcatStats.Timeline timelinePosition, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues)
    {
        lastLoadingContext = null;
        orig(self,game,playerCharacter,timelinePosition,singleRoomWorld,worldName,region,setupValues);
    }
    public static WorldLoader.LoadingContext WARPMAPLOADING = new("WARPMAPLOADING", true);
}
