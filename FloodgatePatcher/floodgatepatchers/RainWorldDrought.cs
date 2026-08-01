using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace FloodgatePatchers
{
    public static class RainWorldDrought
    {
        public static void Patcher(ref AssemblyDefinition assemblyDefinition, string CurrentVersion, bool IsLatestVersion)
        {
            ModuleDefinition module = assemblyDefinition.MainModule;
            ILProcessor IntroRollprocessor = module.GetType("Rain_World_Drought.Resource.DroughtIntroRoll").Methods.FirstOrDefault(i => i.Name == "Patch").Body.GetILProcessor();
            ILProcessor MainMenuPatchprocessor = module.GetType("Rain_World_Drought.DroughtMenu.WandererMainMenu").Methods.FirstOrDefault(i => i.Name == "Patch").Body.GetILProcessor();
            IntroRollprocessor.Body.Instructions.Clear();
            IntroRollprocessor.Emit(OpCodes.Ret);
            MainMenuPatchprocessor.Body.Instructions.Clear();
            MainMenuPatchprocessor.Emit(OpCodes.Ret);
        }

    }
}
