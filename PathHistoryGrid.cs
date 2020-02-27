using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace Madeline.PathOverlay
{
    public class PathHistoryGrid
    {
        public enum Filter
        {
            OnlyPawn,
            PawnAndAnimal,
            OnlyEnemy
        }
        iOverlayDrawer drawer = new CellOverlayDrawer();
        Map map;
        public int[] grid; // 0~1000?
        public int IncrementPerEachVisit { get; set; } = 800;
        Color Green = new Color32(70, 180, 120, 255);
        Color Red = new Color32(180, 50, 50, 255);
        Color BackGround = new Color32(50, 50, 50, 255);
        public PathHistoryGrid(Map map)
        {
            this.map = map;

            ResetPathGrid();

            /*
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                InitializeGridDrawer();
            });
            */
            InitializeGridDrawer();
        }

        public void InitializeGridDrawer()
        { // should be in main thread
            drawer.Initialize(ShouldDrawCellOverlay, GetCellColor, map);
        }

        public void ResetPathGrid()
		{
			this.grid = new int[this.map.cellIndices.NumGridCells];
		}

        public bool ShouldDrawCellOverlay(int cellIndex)
        {
            return true;
        }

        public Color GetCellColor(int cellIndex)
        {
            int value = grid[cellIndex];
            if(value < 800)
            {
                float normalized = (float)value / (float)IncrementPerEachVisit;
                Log.Message("Normalized : " + normalized.ToString());
                return Color.Lerp(BackGround, Green, normalized);
            }
            if(800 <= value)
            { // 5단계까지, 이후는 Max니까,
              // Max = 800 * 5 = 4000, Min = 800, 너비 3200
                float normalized = (float)Mathf.Clamp((value - 800), 0, 3200) / (float)3200;
                return Color.Lerp(Green, Red, normalized); // maxValue
            }
            
            throw new Exception("got unexpected value from target cellIndex");
        }

        public void Notify_PawnVisitedCell(Pawn pawn, IntVec3 cell)
        {
            if(!isAllowedPawnType(pawn))
                return;

            var cellIndex = CellIndicesUtility.CellToIndex(cell, map.Size.x);
            grid[cellIndex] += IncrementPerEachVisit;
            //grid[cellIndex] = Mathf.Clamp(grid[cellIndex], 0, 1000); // 여기에 최대, 최소값이 정의
            Log.Message($"Increment Cell {cell.x} {cell.z}, value : {grid[cellIndex]}");

            //테스트 전용, CellBoolDrawerWrapper일때만
            var casted = drawer as CellBoolDrawerWrapper;
            if(casted != null)
            {
                Log.Message("Re-drawing SetDirty");
                casted.SetDirty();
            }
        }

        bool isAllowedPawnType(Pawn pawn)
        {
            if(pawn.IsColonist)
                return true;
            else
                return false;
        }

        public void PathHistoryGridUpdate()
        {
            drawer.ShouldDraw = OverlayGlobalSettings.DrawPathOverlay;
            drawer.UpdateOverlayDrawer();
        }

        public void PathHistoryGridTick()
        {
            int size = grid.Count();
            for(int i = 0; i < size; i++)
            {
                if(grid[i] > 0)
                {
                    var Cell = CellIndicesUtility.IndexToCell(i, map.Size.x);

                    /*
                    if(grid[i] > 1)
                        drawer.Notify_ColorChanged(Cell.x, Cell.z, new Color32(255, 0, 0, 255));
                    else
                        drawer.Notify_ColorChanged(Cell.x, Cell.z, new Color32(50, 50, 50, 255));
                        */

                    grid[i] -= 1;
                    
                    var CellColor = GetCellColor(i);
                    drawer.Notify_ColorChanged(Cell.x, Cell.z, CellColor);
                    
                }
            }
        }
    }
}

/*
  초당 180틱, 1단계가 내려가는데 걸리는 시간 -> 5초?
  그러면 단계당 180 * 5 = 800틱
*/