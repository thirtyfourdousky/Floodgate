using Menu.Remix.MixedUI;
using UnityEngine;

namespace Floodgate;

public class RemixInterface : OptionInterface
{

    public OpCheckBox _showWorkshopDate;
    public readonly Configurable<bool> ShowWorkshopDate;
    public OpCheckBox _lowBudgetModdedExperience;
    public readonly Configurable<bool> LowBudgetModdedExperience;


    public RemixInterface()
    {
        ShowWorkshopDate = config.Bind("fgshowworkshop", false);
        LowBudgetModdedExperience = config.Bind("fglowbudgetmoddedexperience", false);       
    }

    public override void Initialize()
    {
        Vector2 a = new Vector2(300f, 443f);
        Vector2 b = new Vector2(20f, 450f);
        Vector2 c = new Vector2(0f, -30f);
        OpTab floodgateOptions = new(this, "Remix Options");
        int separator = 0;
        floodgateOptions.AddItems(
            new OpLabel(10, 540, "Floodgate", true) { alignment = FLabelAlignment.Left },
            _lowBudgetModdedExperience = new(LowBudgetModdedExperience, a + c * separator), new OpLabel(b + c * separator++, new(300f, 24f), "Merge Modded Creatures"),
            _showWorkshopDate = new(ShowWorkshopDate, a + c * separator), new OpLabel(b + c * separator++, new(300f,24f), "Workshop Last Update")
        );
        _lowBudgetModdedExperience.description = "Adds some modded creatures to your world";

        OpTab debug = new(this, "Debug");
        separator = 0;
        OpSimpleButton rescan;
        debug.AddItems(
            new OpLabel(10, 540, "Floodgate", true) { alignment = FLabelAlignment.Left },
            rescan = new OpSimpleButton(b + c * separator++, new(300f, 24f), "Rescan Floodgate Paths")//,
            //new OpLabel(b+c*separator, new(300f,24f), "File map: " + TurboAssetManager.DirectoryMap.Count + " - " + TurboAssetManager.UnmappedFiles.Count) { alignment = FLabelAlignment.Left }
            );
        rescan.OnClick += Rescan_OnClick;

        Tabs = [floodgateOptions, debug];
    }

    private void Rescan_OnClick(UIfocusable trigger)
    {
        Registry.OpRescan();
        World.CustomMerger.Rescan();
    }
}
