using Verse;
using HarmonyLib;
using RimWorld;

namespace Madeline.OverlayExtension
{
    public static class FilthGridHarmonyPatch
    {
        public static void DoPatch(Harmony harmony)
        {
            return;
            var original = AccessTools.Method(typeof(Filth), nameof(Filth.ThickenFilth));
            var original2 = AccessTools.Method(typeof(Filth), nameof(Filth.ThinFilth));
            var postfix = AccessTools.Method(typeof(FilthGridHarmonyPatch), nameof(FilthGridHarmonyPatch.FilthThickChangePostfix));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            harmony.Patch(original2, postfix: new HarmonyMethod(postfix));

            var original3 = AccessTools.Method(typeof(Filth), nameof(Filth.SpawnSetup));
            var postfix2 = AccessTools.Method(typeof(FilthGridHarmonyPatch), nameof(FilthGridHarmonyPatch.FilthSpawnedOnMap));
            harmony.Patch(original3, null, new HarmonyMethod(postfix2));
        }

        static void FilthThickChangePostfix(Filth __instance)
        {
            if(__instance.Map == null)
                return; // FilthTracker에 의해 관리되는 Filth임

            //__instance.Map.GetComponent<OverlayMapComponent>().filthGrid.Notify_FilthChanged(__instance.Position, __instance.thickness);
        }
        // Filth가 맵에 등장하는 방식은, 똑같은 Filth를 하나 생성해서 집어넣고 Tracker의 Filth를 한단계 깎는 것
        static void FilthSpawnedOnMap(Filth __instance, Map map)
        { // postfix method, after placing filth on the map.
            var pos = __instance.Position;
            //__instance.Map.GetComponent<OverlayMapComponent>().filthGrid.Notify_FilthChanged(pos, __instance.thickness);
        }
    }
}