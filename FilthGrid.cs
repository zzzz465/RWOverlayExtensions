using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace Madeline.OverlayExtension
{
    public class FilthGrid : iGrid
    {
        public struct ColorChangedRecord
        {
            public int
                x,
                z;
            
            public Color color;
        }
        iOverlayDrawer drawer = new CellOverlayDrawer();
        Map map;
        int[] grid;
        public int maxFilthCount { get; set; } = 5;
        List<ColorChangedRecord> recordCache = new List<ColorChangedRecord>();
        bool wasEnabled = false;
        public bool DebugString = false;
        public FilthGrid(Map map)
        {
            this.map = map;
            InitializeGrid();
        }
        
        void InitializeGrid()
        {
            grid = new int[map.cellIndices.NumGridCells];
            drawer.Initialize(DrawCellOverlay, GetInitialColor, map);
        }

        Color GetInitialColor(int index)
        {
            var intvec3 = CellIndicesUtility.IndexToCell(index, map.Size.x);
            var filth = intvec3.GetThingList(map).Where((param) => param is Filth).FirstOrDefault() as Filth;
            if(filth != null)
            {
                return GetColor(filth.thickness);
            }
            return GetColor(0);
        }

        Color GetColorOfIndex(int index)
        {
            return GetColor(grid[index]);
        }

        Color GetColor(int level)
        {
            switch(level)
            {
                case 0:
                    return new Color32(50, 50, 50, 0);
                
                case 1:
                    return new Color32(180, 200, 80, 255);
                
                case 2:
                    return new Color32(200, 180, 20, 255);
                
                case 3:
                    return new Color32(200, 100, 30, 255);

                case 4:
                    return new Color32(170, 70, 21, 255);
                
                case 5:
                    return new Color32(220, 20, 20, 255);
                
                default:
                    return new Color32(255, 255, 255, 255);
            }
        }

        bool DrawCellOverlay(int index)
        {
            return true;
        }

        public void Notify_FilthChanged(IntVec3 position, int value)
        {
            Log.Message($"ChangeColor {position.x}, {position.z}, val : {value}");
            var index = CellIndicesUtility.CellToIndex(position, map.Size.x);
            grid[index] = Mathf.Clamp(value, 0, maxFilthCount);
            var color = GetColorOfIndex(index);
            if(!drawer.Notify_ColorChanged(position.x, position.z, color))
            {
                recordCache.Add(new ColorChangedRecord() { x = position.x, z = position.z, color = color});
            }
        }

        public void UpdateGrid()
        {
            drawer.ShouldDraw = OverlayGlobalSettings.DrawFilthOverlay;
            drawer.UpdateOverlayDrawer();

            if(!wasEnabled && OverlayGlobalSettings.DrawFilthOverlay)
            {
                FlushOverlayData();
            }

            if(OverlayGlobalSettings.DrawFilthOverlay)
            {
                wasEnabled = true;
            }
            else
            {
                wasEnabled = false;
            }

        }

        void FlushOverlayData()
        {
            if(DebugString)
                Log.Message("Flushing overlay data...", true);
            foreach(var record in recordCache)
            {
                Log.Message($"Changing {record.x}, {record.z} Color to {record.color.ToString()}");
                drawer.Notify_ColorChanged(record.x, record.z, record.color);
            }

            recordCache.Clear();
        }
    }
}