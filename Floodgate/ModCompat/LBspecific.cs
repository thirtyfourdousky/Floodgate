using LBMergedMods.Hooks;
using UnityEngine;

namespace ModCompat;

public static class LBspecific
{
    public static void Apply()
    {
        On.Fly.ctor += Fly_ctor;
    }

    private static void Fly_ctor(On.Fly.orig_ctor orig, Fly self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (string.IsNullOrWhiteSpace(Floodgate.NotEnums.CreatureTemplateType._LBFly.Name) || !Floodgate.Plugin.RemixOptions.LowBudgetModdedExperience.Value)
        {
            return;
        }
        Random.State state = Random.state;
        Random.InitState(abstractCreature.ID.RandomSeed);
        if(Random.value < 0.5f)
        {
            RainWorldGame game = world?.game;
            if(game != null && game.session is not SandboxGameSession && AbstractPhysicalObjectHooks.Seed.TryGetValue(abstractCreature, out var flyProperties))
            {
                flyProperties.IsSeed = true;
                flyProperties.Born = true;
            }
        }
        Random.state = state;
    }
}