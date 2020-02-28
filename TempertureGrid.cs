using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Madeline.OverlayExtension
{
    public class TempertureGrid
    {
        iOverlayDrawer drawer;
        float[] grid;
        Map map;
        public TempertureGrid(Map map)
        {
            this.map = map;
        }

        public void InitializeDrawer()
        {

        }

        public void ResetPathGrid()
        {
            this.grid = new float[map.cellIndices.NumGridCells];
        }
        /*

        public Color GetCellColor(int cellIndex)
        {

        }

        */

        public void UpdateGrid()
        {

        }
    }
}