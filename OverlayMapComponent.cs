using System;
using System.Collections.Generic;
using Verse;

namespace Madeline.PathOverlay
{
    public class OverlayMapComponent : MapComponent
    {
        public PathHistoryGrid pathHistoryGrid;
        public OverlayMapComponent(Map map) : base(map)
        {
            pathHistoryGrid = new PathHistoryGrid(map);
        }

        public override void MapComponentTick()
        {
            pathHistoryGrid.PathHistoryGridTick();
        }

        public override void MapComponentUpdate()
        {
            pathHistoryGrid.PathHistoryGridUpdate();
        }

        public override void MapComponentOnGUI()
        {
            
        }

        public override void FinalizeInit()
        {

        }
    }
}