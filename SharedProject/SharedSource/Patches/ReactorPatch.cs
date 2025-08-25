using Barotrauma;
using Barotrauma.Items.Components;
using HarmonyLib;

namespace ReactorFix.Patches;

[HarmonyPatch(typeof(Reactor), nameof(Reactor.Update))]
public static class ReactorPatch
{
    public static void Postfix(Reactor __instance)
    {
        // Only override when wires are actively controlling it
        if (__instance.signalControlledTargetFissionRate.HasValue &&
            __instance.lastReceivedFissionRateSignalTime > Timing.TotalTime - 1.0)
        {
            __instance.TargetFissionRate = __instance.signalControlledTargetFissionRate.Value;
            __instance.FissionRate = __instance.TargetFissionRate;
        }

        if (__instance.signalControlledTargetTurbineOutput.HasValue &&
            __instance.lastReceivedTurbineOutputSignalTime > Timing.TotalTime - 1.0)
        {
            __instance.TargetTurbineOutput = __instance.signalControlledTargetTurbineOutput.Value;
            __instance.TurbineOutput = __instance.TargetTurbineOutput;
        }
    }
}
