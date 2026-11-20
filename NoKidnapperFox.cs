using BepInEx;
using BepInEx.Logging;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;

namespace NoKidnapperFox;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[LobbyCompatibility(CompatibilityLevel.ServerOnly, VersionStrictness.None)]
public class NoKidnapperFox : BaseUnityPlugin
{
    public static NoKidnapperFox Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    private static void PluginLogInfoWithPrefix(string content)
    {
        Logger.LogInfo($"[EVIL] {content}");
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class LoadSaveData
    {
        [HarmonyPatch("LoadPlanetsMoldSpreadData")]
        [HarmonyPostfix]
        static void ZeroOutMoldData(StartOfRound __instance)
        {
            PluginLogInfoWithPrefix("Zeroing out mold spawn date from the save and setting `canSpawnMold` to false!");
            __instance.levels.ForEach(x =>
            {
                x.moldSpreadIterations = 0;
                x.moldStartPosition = 0;
                x.canSpawnMold = false;
            });
        }
    }
}
