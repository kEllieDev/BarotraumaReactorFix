using Barotrauma;
using Barotrauma.Items.Components;
using System.Reflection;
using HarmonyLib;

namespace ReactorFix.Patches
{
    [HarmonyPatch(typeof(Reactor), nameof(Reactor.Update))]
    public static class ReactorPatch
    {
        // Cache fieldinfo so we don’t do reflection every frame (yucky!!)
        private static readonly FieldInfo? fissionTargetField;
        private static readonly FieldInfo? fissionSignalTimeField;
        private static readonly FieldInfo? turbineTargetField;
        private static readonly FieldInfo? turbineSignalTimeField;

        static ReactorPatch()
        {
            fissionTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetFissionRate");
            fissionSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedFissionRateSignalTime");
            turbineTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetTurbineOutput");
            turbineSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedTurbineOutputSignalTime");

            if (fissionTargetField is null ||
                fissionSignalTimeField is null ||
                turbineTargetField is null ||
                turbineSignalTimeField is null)
            {
                Log.Error("ReactorFix: Failed to locate one or more Reactor private fields!");
                Log.FileLog("ReactorFix: Failed to locate one or more Reactor private fields!");
            }
        }

        public static void Postfix(Reactor __instance)
        {
            try
            {
                if (fissionTargetField is null ||
                    fissionSignalTimeField is null ||
                    turbineTargetField is null ||
                    turbineSignalTimeField is null)
                {
                    return; // We failed to locate the fields, so just skip the patch
                }

                var signalControlledTargetFissionRate = (float?)fissionTargetField.GetValue(__instance);
                var lastReceivedFissionRateSignalTime = (double?)fissionSignalTimeField.GetValue(__instance);
                var signalControlledTargetTurbineOutput = (float?)turbineTargetField.GetValue(__instance);
                var lastReceivedTurbineOutputSignalTime = (double?)turbineSignalTimeField.GetValue(__instance);

                if (signalControlledTargetFissionRate.HasValue &&
                    lastReceivedFissionRateSignalTime > Timing.TotalTime - 1.0)
                {
                    __instance.TargetFissionRate = signalControlledTargetFissionRate.Value;
                    __instance.FissionRate = __instance.TargetFissionRate;
                }

                if (signalControlledTargetTurbineOutput.HasValue &&
                    lastReceivedTurbineOutputSignalTime > Timing.TotalTime - 1.0)
                {
                    __instance.TargetTurbineOutput = signalControlledTargetTurbineOutput.Value;
                    __instance.TurbineOutput = __instance.TargetTurbineOutput;
                }
            }
            catch (Exception e)
            {
                Log.Error($"ReactorFix patch failed: {e}");
                // Don't filelog or we'll spam the log file!!
            }
        }
    }
}
