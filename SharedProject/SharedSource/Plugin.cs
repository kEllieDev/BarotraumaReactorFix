using Barotrauma;
using HarmonyLib;

namespace ReactorFix;

public partial class Plugin : IAssemblyPlugin
{
    private Harmony? _harmony;

    public void Initialize()
    {
        Log.Info("Initialized!");
        _harmony = Patches.Patches.CreateAndApplyAllPatches();
    }

    public void OnLoadCompleted()
    {
    }

    public void PreInitPatching()
    {
        // Unused
    }

    public void Dispose()
    {
        // Clean up if necessary
    }
}