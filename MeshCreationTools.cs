using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RioniUtils
{
    public static class MeshCreationTools
    {
        public static Mesh CreateQuadMeshFromPositions(List<Vector3> tileWorldLocations, bool uvPerTile)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Quad";
            float cellSize = 1;

            int tilesCount = tileWorldLocations.Count;

            Vector3[] newVertices = new Vector3[4 * tilesCount];
            int[] tris = new int[6 * tilesCount];
            Vector3[] normals = new Vector3[4 * tilesCount];
            Vector2[] uvs = new Vector2[4 * tilesCount];

            Bounds bounds = new Bounds();
            if (!uvPerTile)
                for (int i = 0; i < tilesCount; i++)
                    bounds.Encapsulate(tileWorldLocations[i]);

            float leftBound = bounds.min.x;
            float rightBound = bounds.max.x + cellSize;
            float lowerBound = bounds.min.y;
            float upperBound = bounds.max.y + cellSize;

            for (int i = 0; i < tilesCount; i++)
            {
                Vector3 tilePos = tileWorldLocations[i];
                int j = i * 4;
                int k = i * 6;
                newVertices[j] = new Vector3(tilePos.x, tilePos.y, tilePos.z);  //Botton left
                newVertices[j + 1] = new Vector3(tilePos.x + cellSize, tilePos.y, tilePos.z);  //Bottom right
                newVertices[j + 2] = new Vector3(tilePos.x, tilePos.y + cellSize, tilePos.z);  //Top Left
                newVertices[j + 3] = new Vector3(tilePos.x + cellSize, tilePos.y + cellSize, tilePos.z);  //Top Right

                //Introducir triangulos en sentido antihorario
                tris[k] = j + 1;
                tris[k + 1] = j + 3;
                tris[k + 2] = j + 0;
                tris[k + 3] = j + 0;
                tris[k + 4] = j + 3;
                tris[k + 5] = j + 2;

                normals[j] = Vector3.back;
                normals[j + 1] = Vector3.back;
                normals[j + 2] = Vector3.back;
                normals[j + 3] = Vector3.back;

                uvs[j] = uvPerTile ? new Vector2(0, 0) : new Vector2(Mathf.InverseLerp(leftBound, rightBound, newVertices[j].x), Mathf.InverseLerp(lowerBound, upperBound, newVertices[j].y));
                uvs[j + 1] = uvPerTile ? new Vector2(1, 0) : new Vector2(Mathf.InverseLerp(leftBound, rightBound, newVertices[j + 1].x), Mathf.InverseLerp(lowerBound, upperBound, newVertices[j + 1].y));
                uvs[j + 2] = uvPerTile ? new Vector2(0, 1) : new Vector2(Mathf.InverseLerp(leftBound, rightBound, newVertices[j + 2].x), Mathf.InverseLerp(lowerBound, upperBound, newVertices[j + 2].y));
                uvs[j + 3] = uvPerTile ? new Vector2(1, 1) : new Vector2(Mathf.InverseLerp(leftBound, rightBound, newVertices[j + 3].x), Mathf.InverseLerp(lowerBound, upperBound, newVertices[j + 3].y));
            }

            mesh.vertices = newVertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uvs;

            return mesh;
        }

        public static Mesh CreateQuadMeshFromTilemap(Tilemap tilemap, bool uvPerTile)
        {
            List<Vector3> tileWorldLocations = new List<Vector3>();

            foreach (Vector3Int posT in tilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(posT.x, posT.y, posT.z);
                Vector3 place = tilemap.CellToWorld(localPlace);
                if (tilemap.HasTile(localPlace))
                    tileWorldLocations.Add(place);
            }

            return CreateQuadMeshFromPositions(tileWorldLocations, uvPerTile);
        }
    }
}