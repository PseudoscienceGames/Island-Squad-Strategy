using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMesh : MonoBehaviour
{
	public static WorldMesh Instance;
	private void Awake(){ Instance = this; }

	public float noiseScale;
	private List<Vector3> verts = new List<Vector3>();
	private List<int> tris = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();
	private List<Color> colors = new List<Color>();

	public void GenMesh()
	{
		int vertCount = 0;
		foreach (IslandData island in WorldData.Instance.islands)
		{
			foreach (WorldTile tile in island.tiles)
			{
				verts.AddRange(tile.verts);
				uvs.AddRange(tile.uvs);
				foreach (int tri in tile.tris)
				{
					tris.Add(tri + vertCount);
				}
				vertCount += 6;
			}
		}
		ExpandDoubles();
		for (int i = 0; i < tris.Count; i += 3)
		{
			List<float> h = new List<float>
			{
				verts[tris[i]].y / HexGrid.tileHeight,
				verts[tris[i + 1]].y / HexGrid.tileHeight,
				verts[tris[i + 2]].y / HexGrid.tileHeight
			};
			if (Mathf.Approximately(h[0], h[1]) && Mathf.Approximately(h[0], h[2]))
			{
				if (h[0] < 1.5f || h[1] < 1.5f || h[2] < 1.5f)
				{
					colors.Add(new Color(0, 0, 0, 0));
					colors.Add(new Color(0, 0, 0, 0));
					colors.Add(new Color(0, 0, 0, 0));
				}
				else
				{
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
				}
			}
			else
			{
				float l1 = Mathf.Round(Vector3.Distance(verts[tris[i]], verts[tris[i + 1]]) * 1000f) / 1000f;
				float l2 = Mathf.Round(Vector3.Distance(verts[tris[i + 1]], verts[tris[i + 2]]) * 1000f) / 1000f;
				float l3 = Mathf.Round(Vector3.Distance(verts[tris[i + 2]], verts[tris[i]]) * 1000f) / 1000f;
				//Debug.Log(l1);
				//Debug.Log(l2);
				//Debug.Log(l3);
				if (h[0] < 1.5f && h[1] < 1.5f && h[2] < 1.5f)
				{
					colors.Add(new Color(0, 0, 0, 0));
					colors.Add(new Color(0, 0, 0, 0));
					colors.Add(new Color(0, 0, 0, 0));
				}
				else if (
					((Mathf.Approximately(l1, 1.118f) && (Mathf.Approximately(l2, 1.258f) || Mathf.Approximately(l3, 1.258f))) ||
					(l2 == 1.118f && (l1 == 1.258f || l3 == 1.258f)) ||
					(l3 == 1.118f && (l2 == 1.258f || l1 == 1.258f))))
				{
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
				}
				else if(
					(Mathf.Abs(h[0] - h[1]) > 1 || Mathf.Abs(h[0] - h[2]) > 1 || Mathf.Abs(h[1] - h[2]) > 1 ||
					(Mathf.Approximately(l1, 0.629f) && Mathf.Approximately(l2, 0.629f)) ||
					(Mathf.Approximately(l2, 0.629f) && Mathf.Approximately(l3, 0.629f)) ||
					(Mathf.Approximately(l3, 0.629f) && Mathf.Approximately(l1, 0.629f))))


				{
					colors.Add(new Color(1f / 100f, 0, 0, 0));
					colors.Add(new Color(1f / 100f, 0, 0, 0));
					colors.Add(new Color(1f / 100f, 0, 0, 0));
				}
				else
				{
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
					colors.Add(new Color(2f / 100f, 0, 0, 0));
				}
			}
		}

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
		GetComponent<MeshCollider>().sharedMesh = mesh;
		Debug.Log(verts.Count + " " + tris.Count + " " + colors.Count + " " + Time.realtimeSinceStartup);
	}
	void ExpandDoubles()
	{
		List<Vector3> newVerts = new List<Vector3>();
		List<int> newTris = new List<int>();
		List<Vector2> newUVs = new List<Vector2>();
		foreach (int tri in tris)
		{
			newVerts.Add(verts[tri]);
			newTris.Add(newVerts.Count - 1);
			newUVs.Add(uvs[tri]);
		}
		verts = newVerts;
		tris = newTris;
		uvs = newUVs;
	}
}
