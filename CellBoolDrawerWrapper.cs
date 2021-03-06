using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace Madeline.OverlayExtension
{
    public class CellBoolDrawerWrapper : iOverlayDrawer
    {
        public bool ShouldDraw { get; set; }
        CellBoolDrawer drawer;
        bool Initialized = false;

        public void Initialize(Func<int, bool> DrawCellOverlay, Func<int, Color> CellColorGetter, Map map)
        {
            drawer = new CellBoolDrawer(DrawCellOverlay, ColorGetter, CellColorGetter, map.Size.x, map.Size.z, opacity: 0.35f);
            Initialized = true;
        }
        public Color ColorGetter()
        {
            return Color.white;
        }
        public void UpdateOverlayDrawer()
        {
            if(!Initialized)
                throw new Exception("tried to Update Drawer without initializing");

            if(ShouldDraw)
                drawer.MarkForDraw();
            
            drawer.CellBoolDrawerUpdate();
        }

        public void SetDirty()
        {
            drawer.SetDirty();
        }

        public bool Notify_ColorChanged(int x, int z, Color color)
        {
            return false;
            // do nothing.
        }
    }
}