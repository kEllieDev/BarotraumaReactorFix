using Barotrauma.Items.Components;
using HarmonyLib;

namespace ReactorFix.Patches;

[HarmonyPatch( typeof(Reactor), nameof(Reactor.Update) )]
public static class ReactorPatch
{
    public static bool Prefix( Reactor __instance )
    {
        // TODO: Apply Fix
        return true; // Return true to continue with the original method, false to skip it
    }
}