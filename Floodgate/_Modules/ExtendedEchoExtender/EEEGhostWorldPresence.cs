using FloodgatePatcher;

namespace Floodgate._Modules.ExtendedEchoExtender;

internal class EEEGhostWorldPresence : GhostWorldPresence
{
    public EEEGhostWorldPresence(global::World world, GhostID ghostID) : this(world, ghostID, 0)
    {
    }

    public EEEGhostWorldPresence(global::World world, GhostID ghostID, int spinningTopSpawnId) : base(world, ghostID, spinningTopSpawnId)
    {
        if(Registry.echoesSettings.TryGetValue(ghostID, out var echoesSettings))
        {
            this.ghostRoom = world.GetAbstractRoom(echoesSettings.Room);
            this.songName = echoesSettings.Song;
        }
        else
        {
            this.ghostRoom = world.abstractRooms[world.abstractRooms.Length - 1];
        }
    }

    public static new bool SpawnGhost(GhostID ghostID, int karma, int karmaCap, int ghostPreviouslyEncountered, bool playingAsRed)
    {
        if ((ModManager.Expedition && RWCustom.Custom.rainWorld.ExpeditionMode && RWCustom.Custom.rainWorld.progression.currentSaveState.cycleNumber == 0) || RWCustom.Custom.rainWorld.safariMode || !Registry.echoesSettings.TryGetValue(ghostID, out var Echo))
        {
            return false;
        }
        bool karmaMet = Echo.KarmaCondition(karma, karmaCap);
        bool karmaCapMet = karmaCap >= Echo.MinKarmaCap;
        bool priming = (Echo.Priming!= EchoSettings.PrimingType.Regular) ? (ghostPreviouslyEncountered != 2) : (ghostPreviouslyEncountered == 1);
        CustomLog.Log("[Extended Echo] Reading data for " + ghostID.value);
        CustomLog.Log("[Extended Echo] Spawn on current campaign: " + (Echo.SpawnOnDifficulty ? "Met" : "Not Met"));
        CustomLog.Log("[Extended Echo] Minimum Karma: " + (karmaMet ? "Met" : "Not Met") + ", Required: " + (Echo.MinKarma <= -1 ? "Dynamic"  : Echo.MinKarma) + ", Having: " + karma);
        CustomLog.Log("[Extended Echo] Minimum Karma cap: " + (karmaCapMet ? "Met" : "Not Met") + ", Required: " + Echo.MinKarmaCap + ", Having: " + karmaCap);
        CustomLog.Log("[Extended Echo] Echo Song: " + Echo.Song);
        CustomLog.Log("[Extended Echo] Echo Room: " + Echo.Room);
        CustomLog.Log("[Extended Echo] Primed " + (priming ? "Yes" : "No") + ", Required: " + (int)Echo.Priming + ", Having: " + ghostPreviouslyEncountered);
        CustomLog.Log("[Extended Echo] Spawning Echo " + ((karmaMet && karmaCapMet && priming && Echo.SpawnOnDifficulty) ?"Yes" : "No"));
        return karmaMet && karmaCapMet && priming && Echo.SpawnOnDifficulty;
    }
}
