using System;
using ADOFAI;
using ERDPatch;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace EventLib; 

public abstract class Main {
    public static Harmony harmony { get; private set; }
    internal static UnityModManager.ModEntry entry;

    private static bool Load(UnityModManager.ModEntry mod_entry) {
        entry = mod_entry;
            
        //entry.OnGUI = OnGUI;
        harmony = new Harmony(mod_entry.Info.Id);
        harmony.PatchAll();
        return true;
    }

    private static string asdf = "";
    private static string l = "";
    private static ulong k = 100;

    public static bool showInternalEvents = false;
    
    private static void OnGUI(UnityModManager.ModEntry mod_entry) {
        GUILayout.Label("Mod Test");
        GUILayout.Label("Special thanks to C##");
        asdf = GUILayout.TextField(asdf);
        if (GUILayout.Button("Patch")) {
            EnumPatcher<LevelEventType>.AddField(asdf, k++);
        }

        if (GUILayout.Button("Parse")) {
            l = "";
            l += $"{Enum.Parse(typeof(LevelEventType), asdf)}\n";
            l += $"{Enum.Parse<LevelEventType>(asdf).ToString()}\n";
            Enum.TryParse(typeof(LevelEventType), asdf, out var result);
            l += $"{result}\n";
            Enum.TryParse<LevelEventType>(asdf, out var result2);
            l += $"{result2}\n";
        }
        GUILayout.Label(l);
        
        showInternalEvents = GUILayout.Toggle(showInternalEvents, "Show Internal Events");
    }
}