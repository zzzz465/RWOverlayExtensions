using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace Madeline.OverlayExtension
{
    public static class OverlayButtonHarmonyPatch
    {
        static Texture2D PathOverlayIcon;
        static Texture2D filthOverlayIcon;
        public static void DoPatch(Harmony harmony)
        {
            var DoPlaySettingsGlobalControls = AccessTools.Method(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls));
            var AddOverlayButtonToWidgets = AccessTools.Method(typeof(OverlayButtonHarmonyPatch), nameof(OverlayButtonHarmonyPatch.AddOverlayButtonToWidgets));
            harmony.Patch(DoPlaySettingsGlobalControls, null, new HarmonyMethod(AddOverlayButtonToWidgets));

            LongEventHandler.ExecuteWhenFinished(() => {
                PathOverlayIcon = ContentFinder<Texture2D>.Get("Running", true);
                //filthOverlayIcon = ContentFinder<Texture2D>.Get("CleanUp", true);
            });
        }

        static void AddOverlayButtonToWidgets(WidgetRow row)
        { // postfix to PlaySettings.DoPlaySettingsGlobalControls
            row.ToggleableIcon(ref OverlayGlobalSettings.DrawPathOverlay, PathOverlayIcon, "draw Path Overlay", null, null);
            //row.ToggleableIcon(ref OverlayGlobalSettings.DrawFilthOverlay, filthOverlayIcon, "draw Filth Overlay", null, null);
        }
    }
}