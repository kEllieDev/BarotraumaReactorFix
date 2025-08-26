using Barotrauma;
using Barotrauma.Items.Components;
using System.Reflection;
using HarmonyLib;

namespace ReactorFix.Patches;

[HarmonyPatch(typeof(Reactor), nameof(Reactor.Update))]
public static class ReactorPatch
{
    // Cache fieldinfo so we dont do reflection every frame (yucky!!)
    private static readonly FieldInfo? fissionTargetField;
    private static readonly FieldInfo? fissionSignalTimeField;
    private static readonly FieldInfo? turbineTargetField;
    private static readonly FieldInfo? turbineSignalTimeField;
    private static readonly FieldInfo? totalTimeField;

    private static readonly PropertyInfo? targetFissionRateProp;
    private static readonly PropertyInfo? targetTurbineOutputProp;
    private static readonly PropertyInfo? fissionRateProp;
    private static readonly PropertyInfo? turbineOutputProp;

    static ReactorPatch()
    {
        fissionTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetFissionRate");
        fissionSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedFissionRateSignalTime");
        turbineTargetField = AccessTools.Field(typeof(Reactor), "signalControlledTargetTurbineOutput");
        turbineSignalTimeField = AccessTools.Field(typeof(Reactor), "lastReceivedTurbineOutputSignalTime");
        totalTimeField = AccessTools.Field(typeof(Timing), "TotalTime");

        targetFissionRateProp = AccessTools.Property(typeof(Reactor), "TargetFissionRate");
        targetTurbineOutputProp = AccessTools.Property(typeof(Reactor), "TargetTurbineOutput");
        fissionRateProp = AccessTools.Property(typeof(Reactor), "FissionRate");
        turbineOutputProp = AccessTools.Property(typeof(Reactor), "TurbineOutput");

        if (fissionTargetField is null || fissionSignalTimeField is null ||
            turbineTargetField is null || turbineSignalTimeField is null ||
            totalTimeField is null || targetFissionRateProp is null || targetTurbineOutputProp is null ||
            fissionRateProp is null || turbineOutputProp is null)
        {
            Log.Error("ReactorFix: Failed to locate one or more Reactor fields/properties!");
            Log.FileLog("ReactorFix: Failed to locate one or more Reactor fields/properties!");
        }
    }

    public static void Postfix(Reactor __instance)
    {
        try
        {
            if (fissionTargetField is null || fissionSignalTimeField is null ||
                turbineTargetField is null || turbineSignalTimeField is null ||
                totalTimeField is null || targetFissionRateProp is null || targetTurbineOutputProp is null ||
                fissionRateProp is null || turbineOutputProp is null)
            {
                Log.Error("ReactorFix: Failed to locate one or more Reactor fields/properties!");
                return;
            }

            var signalControlledTargetFissionRate = (float?)fissionTargetField.GetValue(__instance);
            var lastReceivedFissionRateSignalTime = (double?)fissionSignalTimeField.GetValue(__instance);
            var signalControlledTargetTurbineOutput = (float?)turbineTargetField.GetValue(__instance);
            var lastReceivedTurbineOutputSignalTime = (double?)turbineSignalTimeField.GetValue(__instance);

            double? totalTime = (double?)totalTimeField.GetValue(null);
            if (totalTime is null)
            {
                Log.Error("ReactorFix: Failed to locate TotalTime field!");
                return;
            }

            if (signalControlledTargetFissionRate.HasValue && lastReceivedFissionRateSignalTime > totalTime.Value - 1.0)
            {
                targetFissionRateProp.SetValue(__instance, signalControlledTargetFissionRate.Value);
                fissionRateProp.SetValue(__instance, signalControlledTargetFissionRate.Value);
            }

            if (signalControlledTargetTurbineOutput.HasValue && lastReceivedTurbineOutputSignalTime > totalTime.Value - 1.0)
            {
                targetTurbineOutputProp.SetValue(__instance, signalControlledTargetTurbineOutput.Value);
                turbineOutputProp.SetValue(__instance, signalControlledTargetTurbineOutput.Value);
            }
        }
        catch (Exception e)
        {
            Log.Error($"ReactorFix patch failed: {e}");
            // Don't filelog or we'll spam the log file!!
        }
    }
}
