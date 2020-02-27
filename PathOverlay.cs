using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Madeline.PathOverlay
{
    public class PathOverlay : Mod
    {
        public PathOverlay(ModContentPack pack) : base(pack)
        {
            Log.Message("Initializing Madeline's PathOverlay...");
            DoHarmonyPatch();
        }

        void DoHarmonyPatch()
        {
            var harmony = new Harmony("Madeline.PathOverlay");
            HarmonyPatches.DoPatch(harmony);
        }
    }
}