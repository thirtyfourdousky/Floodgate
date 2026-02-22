using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate._Modules.DevTools.Objects;

public static class ObjectsMenu
{
    internal static void Enable()
    {
        On.DevInterface.ObjectsPage.DevObjectGetCategoryFromPlacedType += ObjectsPage_DevObjectGetCategoryFromPlacedType;
    }

    private static DevInterface.ObjectsPage.DevObjectCategories ObjectsPage_DevObjectGetCategoryFromPlacedType(On.DevInterface.ObjectsPage.orig_DevObjectGetCategoryFromPlacedType orig, DevInterface.ObjectsPage self, PlacedObject.Type type)
    {
        DevInterface.ObjectsPage.DevObjectCategories res = orig(self,type);

        if(type == ExtendedEchoExtender.Enums.EEEGhostSpot)
        {
            res = DevInterface.ObjectsPage.DevObjectCategories.Gameplay;
        }

        return res;
    }
}
