using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace Madeline.PathOverlay
{
    public interface iOverlayDrawer
    {
        bool ShouldDraw { set; }
        void Initialize(Func<int, bool> DrawCellOverlay, Func<int, Color> CellColorGetter, Map map);
        void UpdateOverlayDrawer();
        void Notify_ColorChanged(int x, int z, Color color);
    }
}