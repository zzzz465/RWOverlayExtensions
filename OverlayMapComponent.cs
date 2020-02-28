using System;
using System.Collections.Generic;
using Verse;

namespace Madeline.OverlayExtension
{
    public class OverlayMapComponent : MapComponent
    {
        public PathHistoryGrid pathHistoryGrid;
        public FilthGrid filthGrid;
        public OverlayMapComponent(Map map) : base(map)
        {
            pathHistoryGrid = new PathHistoryGrid(map);
            filthGrid = new FilthGrid(map);
        }

        public override void MapComponentTick()
        {
            pathHistoryGrid.PathHistoryGridTick();
        }

        public override void MapComponentUpdate()
        {
            pathHistoryGrid.UpdateGrid();
            filthGrid.UpdateGrid();
        }

        public override void MapComponentOnGUI()
        {
            
        }

        public override void FinalizeInit()
        {

        }
    }
}