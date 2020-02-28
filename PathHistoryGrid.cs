using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace Madeline.PathOverlay
{
    public class PathHistoryGrid : iGrid
    {
        public enum Filter
        {
            OnlyPawn,
            PawnAndAnimal,
            OnlyEnemy
        }
        iOverlayDrawer drawer = new CellOverlayDrawer();
        Map map;
        public float[] grid; // 0~1000?
        public float IncrementPerEachVisit { get; set; } = 800;
        public float MaxAccumulatedVisit { get; set; } = 8000;
        public float DecrementPerTick { get; set; } = 0.5f;
        public float GreenColorBoundary { get; set; } = 1600;
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
			this.grid = new float[this.map.cellIndices.NumGridCells];
		}

        public bool ShouldDrawCellOverlay(int cellIndex)
        {
            return true;
        }

        public Color GetCellColor(int cellIndex)
        {
            float value = grid[cellIndex];
            if(value < GreenColorBoundary)
            { // 배경 -> 초록색 넘어가는 구간
                float normalized = (float)value / (float)GreenColorBoundary;
                return Color.Lerp(BackGround, Green, normalized);
            }
            if(800 <= value)
            {
                /*
                  계산식 : 0부터 첫 방문 1단계까지는 배경 -> 초록색으로 Lerp을 하고
                          1단계부터는 초록색-빨간색 Lerp을 해야하니 이전 값을 제외한 상태에서 Lerp 계산을 해야함.
                */
                float normalized = (float)Mathf.Clamp((value - GreenColorBoundary), 0, MaxAccumulatedVisit - GreenColorBoundary) / (float)(MaxAccumulatedVisit - GreenColorBoundary);
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
            grid[cellIndex] = Mathf.Clamp(grid[cellIndex], 0, MaxAccumulatedVisit);
            //grid[cellIndex] = Mathf.Clamp(grid[cellIndex], 0, 1000); // 여기에 최대, 최소값이 정의
            //Log.Message($"Increment Cell {cell.x} {cell.z}, value : {grid[cellIndex]}");
        }

        bool isAllowedPawnType(Pawn pawn)
        {
            if(pawn.IsColonist)
                return true;
            else
                return false;
        }

        public void UpdateGrid()
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

                    grid[i] -= DecrementPerTick;
                    
                    var CellColor = GetCellColor(i);
                    drawer.Notify_ColorChanged(Cell.x, Cell.z, CellColor);   
                }
                else if(grid[i] < 0)
                {
                    var Cell = CellIndicesUtility.IndexToCell(i, map.Size.x);
                    grid[i] = 0;
                    drawer.Notify_ColorChanged(Cell.x, Cell.z, BackGround);
                }
            }
        }
    }
}

/*
  초당 180틱, 1단계가 내려가는데 걸리는 시간 -> 5초?
  그러면 단계당 180 * 5 = 800틱
*/