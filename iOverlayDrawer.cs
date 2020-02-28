using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace Madeline.OverlayExtension
{
    public interface iOverlayDrawer
    {
        bool ShouldDraw { set; }
        void Initialize(Func<int, bool> DrawCellOverlay, Func<int, Color> CellInitialColorGetter, Map map);
        void UpdateOverlayDrawer();
        bool Notify_ColorChanged(int x, int z, Color color);
    }
}