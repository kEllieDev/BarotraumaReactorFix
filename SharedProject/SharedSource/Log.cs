using Barotrauma;
using Barotrauma.Networking;
using Microsoft.Xna.Framework;
using HarmonyLib;
using System.Reflection;

namespace ReactorFix;

/// <summary>
/// Utility logger class.
/// </summary>
public static class Log
{
    private static readonly string LogPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/LocalMods/ReactorFix/ReactorFix_log.txt");

    public static void Info(string message, ServerLog.MessageType messageType = ServerLog.MessageType.ServerMessage)
    {
        ConsoleLog(message, Color.White, messageType);
    }

    public static void Warning(string message, ServerLog.MessageType messageType = ServerLog.MessageType.ServerMessage)
    {
        ConsoleLog(message, Color.Yellow, messageType);
    }

    public static void Error(string message)
    {
        ConsoleLog(message, Color.Red, ServerLog.MessageType.Error);
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

    private static void ConsoleLog(string message, Color color = new Color(),
        ServerLog.MessageType messageType = ServerLog.MessageType.ServerMessage)
    {
        try
        {
            var method = AccessTools.Method(typeof(LuaCsLogger), "Log",
                [typeof(string), typeof(Color?), typeof(ServerLog.MessageType)]);
            method.Invoke(null, [$"[{Assembly.GetExecutingAssembly().FullName}]: {message}", color, messageType]);
        }
        catch (Exception ex)
        {
            FileLog($"Console Log failed: {ex}");
        }
    }
}