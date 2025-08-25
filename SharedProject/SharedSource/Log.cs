using Barotrauma;
using Barotrauma.Networking;
using Microsoft.Xna.Framework;
using HarmonyLib;

namespace ReactorFix;

/// <summary>
/// Utility logger class.
/// </summary>
public static class Log
{
    private static readonly string LogPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/LocalMods/ReactorFix/ReactorFix_log.txt");

    public static void Info(string message)
    {
        ConsoleLog(message, Color.White);
    }

    public static void Warning(string message)
    {
        ConsoleLog(message, Color.Yellow);
    }

    public static void Error(string message)
    {
        ConsoleLog(message, Color.Red);
    }

    public static void FileLog(object message)
    {
        try
        {
            File.AppendAllText(LogPath, DateTime.UtcNow.ToString("o") + " " + message + Environment.NewLine);
        }
        catch
        {
            // swallow, logging must never throw
        }
    }

    private static void ConsoleLog(string message, Color color = new Color())
    {
        try
        {
            var method = AccessTools.Method(typeof(LuaCsLogger), "Log",
                [typeof(string), typeof(Color?), typeof(ServerLog.MessageType)]);
            method.Invoke(null, [$"[ReactorFix]: {message}", color, ServerLog.MessageType.ServerMessage]);
        }
        catch (Exception ex)
        {
            FileLog($"Console Log failed: {ex}");
        }
    }
}