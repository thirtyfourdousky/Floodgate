using RandomBuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
