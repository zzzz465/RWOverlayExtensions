using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;
using UnityEngine;

namespace Madeline.OverlayExtension
{
    public static class PathHistoryGridHarmonyPatch
    {
        static Pawn currentNodeCalculatingPawn;
        public static void DoPatch(Harmony harmony)
        {
            var SetupMoveIntoNextCell = AccessTools.Method(typeof(Pawn_PathFollower), "SetupMoveIntoNextCell");
            var SetupMoveIntoNextCell_Prefix = AccessTools.Method(typeof(PathHistoryGridHarmonyPatch), nameof(GrabNodeCalculatingPawn));
            harmony.Patch(SetupMoveIntoNextCell, new HarmonyMethod(SetupMoveIntoNextCell_Prefix));

            var ConsumeNextNode_Original = AccessTools.Method(typeof(PawnPath), nameof(PawnPath.ConsumeNextNode));
            var ConsumeNextNode_Prefix = AccessTools.Method(typeof(PathHistoryGridHarmonyPatch), nameof(GrabConsumedNode));
            harmony.Patch(ConsumeNextNode_Original, new HarmonyMethod(ConsumeNextNode_Prefix));
        }

        static void GrabNodeCalculatingPawn(Pawn_PathFollower __instance, Pawn ___pawn)
        { // 필요없지않나? X, 맵마다 Grid의 인스턴스가 따로 있으므로... 찾아줘야함.
            currentNodeCalculatingPawn = ___pawn;
        }

        static void GrabConsumedNode(PawnPath __instance)
        { // prefix로 두는게 나아보인다. 다음 노드를 Consume 했다는 건 현재 노드에 도착했다는 뜻이니까?
            var result = __instance.Peek(0);
            var overlayMapComp = currentNodeCalculatingPawn.Map.GetComponent<OverlayMapComponent>();
            overlayMapComp.pathHistoryGrid.Notify_PawnVisitedCell(currentNodeCalculatingPawn, result);
        }
    }
}