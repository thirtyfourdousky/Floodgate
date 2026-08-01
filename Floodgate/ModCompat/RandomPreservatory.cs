using RandomBuff;

namespace ModCompat;

public static class RandomPreservatory
{
    public static void Apply()
    {
        Floodgate.World.CustomMerger.AddCondition("BuffDisPV", RandomBuffSession);
    }

    public static bool RandomBuffSession(World _)
    {
        return RWCustom.Custom.rainWorld.BuffMode();
    }
}
