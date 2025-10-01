using Barotrauma;
using HarmonyLib;
using System.Reflection;

namespace ReactorFix;

public partial class Plugin : IAssemblyPlugin
{
    public void Initialize()
    {
        Log.Info("Initialized, Creating new harmony instance and patching all...");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
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