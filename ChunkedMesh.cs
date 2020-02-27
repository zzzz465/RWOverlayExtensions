using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;

namespace Madeline.PathOverlay
{
    public class ChunkedMesh
    {
        public Mesh mesh;
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();
        Dictionary<Pair<int, int>, int> coordIndex = new Dictionary<Pair<int, int>, int>();
        bool ShouldReconstructFullMesh = false;
        public ChunkedMesh()
        {
            mesh = new Mesh();
            mesh.name = "ChunkedMesh";
        }

        public void AddCell(int x, int z, Color color)
        { // size = 1
            coordIndex.Add(new Pair<int, int>(x, z), verts.Count);

            float y = AltitudeLayer.MapDataOverlay.AltitudeFor();
            verts.Add(new Vector3((float)x, y, (float)z));
            verts.Add(new Vector3((float)x, y, (float)(z+1)));
            verts.Add(new Vector3((float)(x+1), y, (float)(z+1)));
            verts.Add(new Vector3((float)(x+1), y, (float)z));

            colors.Add(color);
            colors.Add(color);  
            colors.Add(color);
            colors.Add(color);

            int count = verts.Count;
            tris.Add(count - 4);
            tris.Add(count - 3);
            tris.Add(count - 2);
            tris.Add(count - 4);
            tris.Add(count - 2);
            tris.Add(count - 1);
        }

        public bool ChangeColor(int x, int z, Color color)
        {
            var key = new Pair<int, int>(x, z);
            if(coordIndex.ContainsKey(key))
            {
                //Debug
                //var orig_Color = colors[coordIndex[key]];
                //Log.Message($"Changing {x}, {z} color r {orig_Color.r} g {orig_Color.g} b {orig_Color.b} to color r {color.r} g {color.g} b {color.b}");
                //

                var index = coordIndex[key]; // 여기서부터 +0, +1, +2, +3임
                for(int i = 0; i < 4; i++)
                {
                    colors[index + i] = color;
                }
                ShouldReconstructFullMesh = true;

                return true;
            }
            else
                return false;
        }

        public void UpdateChunkedMesh()
        {
            if(ShouldReconstructFullMesh)
            {
                ReconstructFullMesh();
            }

            ShouldReconstructFullMesh = false;
        }

        public void ReconstructFullMesh()
        {
            if(verts.Count > 0)
            {
                mesh.Clear();
                Log.Message("Reconstructing mesh...");
                mesh.SetVertices(verts);
                mesh.SetTriangles(tris, 0);
                mesh.SetColors(colors);
            }
        }
    }
}