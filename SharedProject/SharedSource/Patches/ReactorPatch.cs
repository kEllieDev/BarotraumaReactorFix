using Barotrauma;
using Barotrauma.Items.Components;
using System.Reflection;
using HarmonyLib;

namespace ReactorFix.Patches;

[HarmonyPatch(typeof(Reactor), nameof(Reactor.Update))]
public static class ReactorPatch
{
    // Cache these so we don't do reflection every frame (yucky!!)
    private static readonly FieldInfo? FissionTargetField;
    private static readonly FieldInfo? FissionSignalTimeField;
    private static readonly FieldInfo? TurbineTargetField;
    private static readonly FieldInfo? TurbineSignalTimeField;
    private static readonly FieldInfo? TotalTimeField;

    private static readonly PropertyInfo? TargetFissionRateProp;
    private static readonly PropertyInfo? TargetTurbineOutputProp;

    static ReactorPatch()
    {
        FissionTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetFissionRate");
        FissionSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedFissionRateSignalTime");
        TurbineTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetTurbineOutput");
        TurbineSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedTurbineOutputSignalTime");
        TotalTimeField = AccessTools.Field(typeof(Timing), "TotalTime");

        TargetFissionRateProp = AccessTools.Property(typeof(Reactor), "TargetFissionRate");
        TargetTurbineOutputProp = AccessTools.Property(typeof(Reactor), "TargetTurbineOutput");

        if (FissionTargetField is null || FissionSignalTimeField is null || TurbineTargetField is null ||
            TurbineSignalTimeField is null || TotalTimeField is null || TargetFissionRateProp is null ||
            TargetTurbineOutputProp is null)
        {
            Log.Error("Failed to locate one or more Reactor fields/properties for constructor!");
            Log.FileLog("Failed to locate one or more Reactor fields/properties for constructor!");
        }
    }

    // Intended due to Harmony
    // ReSharper disable once InconsistentNaming
    public static void Postfix(Reactor __instance)
    {
        try
        {
            if (FissionTargetField is null || FissionSignalTimeField is null || TurbineTargetField is null ||
                TurbineSignalTimeField is null || TotalTimeField is null || TargetFissionRateProp is null ||
                TargetTurbineOutputProp is null)
            {
                Log.Error("Failed to locate one or more Reactor fields/properties for PostFix!");
                return;
            }

            var signalControlledTargetFissionRate = (float?)FissionTargetField.GetValue(__instance);
            var lastReceivedFissionRateSignalTime = (double?)FissionSignalTimeField.GetValue(__instance);
            var signalControlledTargetTurbineOutput = (float?)TurbineTargetField.GetValue(__instance);
            var lastReceivedTurbineOutputSignalTime = (double?)TurbineSignalTimeField.GetValue(__instance);

            var totalTime = (double?)TotalTimeField.GetValue(null);
            if (totalTime is null)
            {
                Log.Error("Failed to locate TotalTime field for PostFix!");
                return;
            }

            if (signalControlledTargetFissionRate.HasValue && lastReceivedFissionRateSignalTime > totalTime.Value - 1.0)
            {
                TargetFissionRateProp.SetValue(__instance, signalControlledTargetFissionRate.Value);
            }

            if (signalControlledTargetTurbineOutput.HasValue &&
                lastReceivedTurbineOutputSignalTime > totalTime.Value - 1.0)
            {
                TargetTurbineOutputProp.SetValue(__instance, signalControlledTargetTurbineOutput.Value);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Postfix failed: {e}");
            // Don't file log, or we'll spam the log file!!
        }
    }
}