using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Floodgate.World;

public static class Map
{
    internal static void Apply()
    {
        IL.ModManager.GenerateMergedMods += ModManager_GenerateMergedMods;

        //IL.HUD.Map.Update += Map_Update;
    }

    private static void ModManager_GenerateMergedMods(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            //first go-to just exists to make sure it's the right local
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdstr("mergedmods"), x => x.MatchCallOrCallvirt(out _), x => x.MatchStloc(0)) && c.TryGotoNext(MoveType.After, x => x.MatchStloc(2)))
            {
                IEnumerable<ILLabel> incoming = c.IncomingLabels;
                c.Emit(OpCodes.Ldloc_0);
                foreach (ILLabel label in incoming)
                {
                    label.Target = c.Prev;
                }
                c.EmitDelegate(static delegate (string MergedMods)
                {
                    //literally vanilla (no DLC)
                    string VanillaWorldPath = (RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "World");
                    RelativeCopy(VanillaWorldPath, MergedMods);

                    //dlc and mods
                    for (int i = 0; i < ModManager.ActiveMods.Count; i++)
                    {
                        List<string> searchPaths = new List<string>();

                        string targetedWorld = (ModManager.ActiveMods[i].TargetedPath + Path.DirectorySeparatorChar + "World");
                        if (Directory.Exists(targetedWorld))
                        {
                            searchPaths.Add(targetedWorld);
                        }

                        string newestWorld = (ModManager.ActiveMods[i].NewestPath + Path.DirectorySeparatorChar + "World");
                        if (FloodgatePatcher.ModLoader.IsLatest && Directory.Exists(newestWorld))
                        {
                            searchPaths.Add(newestWorld);
                        }

                        string regularWorld = (ModManager.ActiveMods[i].path + Path.DirectorySeparatorChar + "World");
                        if (Directory.Exists(regularWorld))
                        {
                            searchPaths.Add(regularWorld);
                        }
                        foreach (string path in searchPaths)
                        {
                            RelativeCopy(path, MergedMods);
                        }

                    }

                });
            }
            else
            {
                FloodgatePatcher.CustomLog.LogError("GenerateMergedMods IL hook failed");
            }

        }
        catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError(ex.ToString());
        }
    }
    public static void RelativeCopy(string SourcePath, string MergedMods)
    {
        foreach (string Map in Directory.EnumerateFiles(SourcePath, "map_*.*", SearchOption.AllDirectories))
        {
            if(!(Map.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || Map.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            string destination = (MergedMods + Path.DirectorySeparatorChar + "world" + Path.DirectorySeparatorChar + Map.Replace(SourcePath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, " + Map);
            if (!File.Exists(destination))
            {
                //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, destination " + destination + " does not exists and should be copied");
                try
                {
                    string dir = Path.GetDirectoryName(destination);
                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.Copy(Map, destination);
                }catch (Exception ex)
                {
                    FloodgatePatcher.CustomLog.LogError("[Map \"merging\"] Copying file " + Map + " to " + destination + " failed\n" + ex.ToString());
                }
            }
            else
            {
                //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, destination " + destination + " exists and should be skipped");
            }
        }
    }


    /*private static void Map_Update(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            bool found = false;
            while(c.TryGotoNext(x=>x.MatchCallOrCallvirt(typeof(AssetManager), "SafeWWWLoadTexture")))
            {
                found = true;
                c.Remove();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<delLoadMapTextureOverride>(LoadMapTextureOverride);
            }
            if (!found)
            {
                throw new KeyNotFoundException("Could not find any calls to SafeWWWLoadTexture on Map.Update");
            }
        }
        catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError("[Map \"merging\"] Map rerender apply failed\n" + ex.ToString());
        }
    }

    public delegate Texture2D delLoadMapTextureOverride(ref Texture2D texture2D, string path, bool clampWrapMode, bool crispPixels, HUD.Map map);
    public static Texture2D LoadMapTextureOverride(ref Texture2D texture2D, string _, bool clampWrapMode, bool crispPixels, HUD.Map map)
    {
        DevUI fakeui = new(map.mapData.world.game);
        fakeui.SwitchPage(3);
        MapPage page = fakeui.activePage as MapPage;
        page.canonView = true;
        page.rippleStreamEdit = false;
        page.NewMode();
        page.Refresh();
        page.Update();

        while(page.subNodes.Where(x=>x is RoomPanel).Any(i=>!(i as RoomPanel).miniMap.textureLoaded))
        {

            foreach(var node in page.subNodes)
            {
                node.Update();
                node.Refresh();
                if(node is RoomPanel roompanel && !roompanel.miniMap.textureLoaded)
                {
                    roompanel
                }
            }
            page.Update();
            page.Refresh();
            fakeui.Update();
        }

        page.subNodes.Add(page.renderOutput = new MapRenderOutput(page.owner, page.world, "Render_Output", page, new Vector2(20f, 20f), "Rendered Map", page));
        page.Refresh();
        page.Update();

        texture2D.wrapMode = ((!clampWrapMode) ? TextureWrapMode.Repeat : TextureWrapMode.Clamp);
        if (crispPixels)
        {
            texture2D.anisoLevel = 0;
            texture2D.filterMode = FilterMode.Point;
        }

        Texture2D texture2 = new(page.renderOutput.texture.width, page.renderOutput.texture.height);
        texture2.SetPixels32(page.renderOutput.texture.GetPixels32());
        texture2.Apply(false);

        texture2D.LoadImage(texture2.EncodeToPNG());
        UnityEngine.Object.Destroy(texture2);

        texture2D = page.renderOutput.texture;
        fakeui.ClearSprites();
        return texture2D;
    }*/
}
