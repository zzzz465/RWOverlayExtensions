using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace Madeline.OverlayExtension
{
    public static class OverlayButtonHarmonyPatch
    {
        public static void DoPatch(Harmony harmony)
        {
            var DoPlaySettingsGlobalControls = AccessTools.Method(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls));
            var AddOverlayButtonToWidgets = AccessTools.Method(typeof(OverlayButtonHarmonyPatch), nameof(OverlayButtonHarmonyPatch.AddOverlayButtonToWidgets));
            harmony.Patch(DoPlaySettingsGlobalControls, null, new HarmonyMethod(AddOverlayButtonToWidgets));
        }

        static void AddOverlayButtonToWidgets(WidgetRow row)
        { // postfix to PlaySettings.DoPlaySettingsGlobalControls
            row.ToggleableIcon(ref OverlayGlobalSettings.DrawPathOverlay, new Texture2D(1, 1), "draw Path Overlay", null, null);
            row.ToggleableIcon(ref OverlayGlobalSettings.DrawFilthOverlay, new Texture2D(1, 1), "draw Filth Overlay", null, null);
        }
    }
}