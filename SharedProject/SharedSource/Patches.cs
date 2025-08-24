using HarmonyLib;
using System.Reflection;

namespace ReactorFix.Patches;

public static class Patches
{
    public static Harmony CreateAndApplyAllPatches()
    {
        var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Log.Info("Patching all!");
        return harmony;
    }
}