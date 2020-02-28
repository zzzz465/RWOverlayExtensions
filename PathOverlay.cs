using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace Madeline.OverlayExtension
{
    public class OverlayExtension : Mod
    {
        public OverlayExtension(ModContentPack pack) : base(pack)
        {
            Log.Message("Initializing Madeline's OverlayExtension...");
            DoHarmonyPatch();
        }

        void DoHarmonyPatch()
        {
            var harmony = new Harmony("Madeline.PathOverlay");
            PathHistoryGridHarmonyPatch.DoPatch(harmony);
            FilthGridHarmonyPatch.DoPatch(harmony);
            OverlayButtonHarmonyPatch.DoPatch(harmony);
        }
    }
}