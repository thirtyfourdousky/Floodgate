using System;
using System.Collections.Generic;
using Steamworks;

namespace Floodgate.Steam;

public static class Workshop
{
    public static readonly Dictionary<ulong, DateTime> ModLastUpdatedDT = new Dictionary<ulong, DateTime>();
    private static CallResult<SteamUGCQueryCompleted_t> queryCallback;
    private static UGCQueryHandle_t lastQueryHandle;
    public static void Apply()
    {
        if (SteamManager.Initialized)
        {
            queryCallback = CallResult<SteamUGCQueryCompleted_t>.Create(OnQueryResult);

            AppId_t rwID = new AppId_t(RainWorldSteamManager.APP_ID);
            PublishedFileId_t[] modIds = FilterMods();
            lastQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(modIds, (uint)modIds.Length);
            queryCallback.Set(SteamUGC.SendQueryUGCRequest(lastQueryHandle));
        }
    }

    public static PublishedFileId_t[] FilterMods()
    {
        List<PublishedFileId_t> mods = new();
        foreach(var mod in ModManager.InstalledMods)
        {
            if(mod.workshopMod && mod.workshopId > 0ul)
            {
                mods.Add((PublishedFileId_t)mod.workshopId);
            }
        }
        return [.. mods];
    }

    public static void TryFetch()
    {
        if(SteamManager.Initialized && ModLastUpdatedDT.Count == 0)
        {
            AppId_t rwID = new AppId_t(RainWorldSteamManager.APP_ID);
            PublishedFileId_t[] modIds = FilterMods();
            lastQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(modIds, (uint)modIds.Length);
            queryCallback.Set(SteamUGC.SendQueryUGCRequest(lastQueryHandle));
        }
    }

    public static void OnQueryResult(SteamUGCQueryCompleted_t callback, bool ioFailure)
    {
        if(callback.m_eResult == EResult.k_EResultOK)
        {
            for(uint i = 0u; i < callback.m_unTotalMatchingResults; i++)
            {
                if(SteamUGC.GetQueryUGCResult(lastQueryHandle, i, out var pDetails))
                {
                    ModLastUpdatedDT.Add((ulong)pDetails.m_nPublishedFileId, DateTimeOffset.FromUnixTimeSeconds(pDetails.m_rtimeUpdated).UtcDateTime);
                }
            }
        }
    }
}
