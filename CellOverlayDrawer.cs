using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace Madeline.PathOverlay
{
    public class CellOverlayDrawer : iOverlayDrawer
    {
		Material material;
		bool materialCaresAboutVertexColors;
		float opacity { get; set; } = 0.33f;
		public Color BackgroundColor { get; set; } = new Color32(50, 50, 50, 255);
        public bool ShouldDraw { get; set; }
        Func<int, bool> DrawCellOverlay;
        Func<int, Color> CellColorGetter;
		List<ChunkedMesh> chunkedMeshes = new List<ChunkedMesh>();
        Map map;
		bool chunkedMeshesGenerated = false;
        public void Initialize(Func<int, bool> DrawCellOverlay, Func<int, Color> CellColorGetter, Map map)
        {
            this.DrawCellOverlay = DrawCellOverlay;
            this.CellColorGetter = CellColorGetter;
            this.map = map;
			this.BackgroundColor = BackgroundColor;
			this.opacity = opacity;
        }

		void GenerateChunkedMeshes()
		{
			//Log.Message("Generating Chunk Meshes");
			CellRect cellRect = new CellRect(0, 0, map.Size.x, map.Size.z);
			chunkedMeshes.Add(new ChunkedMesh());
			int curChunkedMeshIndex = 0;
			int AddedCellCountPerChunk = 0;

			var currentChunkedMesh = chunkedMeshes[curChunkedMeshIndex];
			
			for(int x = cellRect.minX; x <= cellRect.maxX; x++)
			{
				for(int z = cellRect.minZ; z <= cellRect.maxZ; z++)
				{
					int CellIndex = CellIndicesUtility.CellToIndex(x, z, map.Size.x);
					var cellColor = CellColorGetter(CellIndex);
					currentChunkedMesh.AddCell(x, z, cellColor);
					AddedCellCountPerChunk++;

					if(AddedCellCountPerChunk >= 16383)
					{
						currentChunkedMesh.ReconstructFullMesh();
						chunkedMeshes.Add(new ChunkedMesh());
						curChunkedMeshIndex += 1;
						AddedCellCountPerChunk = 0;
						currentChunkedMesh = chunkedMeshes[curChunkedMeshIndex];
					}
				}
			}
			currentChunkedMesh.ReconstructFullMesh();
			CreateMaterialIfNeeded(true);
		}

		public void Notify_ColorChanged(int x, int z, Color color)
		{
			foreach(var chunk in chunkedMeshes)
			{
				if(chunk.ChangeColor(x, z, color))
					return;
			}

			Log.Error("None of ChunkedMesh can handle the notify request!");
		}

        public void UpdateOverlayDrawer()
        {
			if(!chunkedMeshesGenerated)
			{
				GenerateChunkedMeshes();
				chunkedMeshesGenerated = true;
			}

			foreach(var chunk in chunkedMeshes)
			{
				chunk.UpdateChunkedMesh();
			}

			if(ShouldDraw)
			{
				foreach(var mesh in chunkedMeshes.Select(chunk => chunk.mesh))
				{
					Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
				}
			}
        }

	
		private void CreateMaterialIfNeeded(bool careAboutVertexColors)
		{
			if (this.material == null || this.materialCaresAboutVertexColors != careAboutVertexColors)
			{
				Color color = Color.white;
				//Color color = new Color32(255, 255, 255, 255); // white Background를 해주는게 그냥 편함.
				//Color color = new Color32(255, 0, 0, 255);
				this.material = SolidColorMaterials.SimpleSolidColorMaterial(new Color(color.r, color.g, color.b, this.opacity * color.a), careAboutVertexColors);
				this.materialCaresAboutVertexColors = careAboutVertexColors;
				this.material.renderQueue = 3600;
			}
		}
    }
}