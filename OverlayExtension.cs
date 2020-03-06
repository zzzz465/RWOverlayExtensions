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
            var harmony = new Harmony("Madeline.OverlayExtension");
            PathHistoryGridHarmonyPatch.DoPatch(harmony);
            FilthGridHarmonyPatch.DoPatch(harmony);
            OverlayButtonHarmonyPatch.DoPatch(harmony);
        }

        /*
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard standard = new Listing_Standard();
            standard.Begin(inRect);
            standard.
        }
        */
    }
}