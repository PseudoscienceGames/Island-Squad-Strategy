using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldTile
{
	public Vector2Int gridLoc;
	public List<int> heights;
	public List<Vector3> verts;
	public List<int> tris;
	public List<Vector2> uvs;
	public List<float> dis = new List<float>();
	public List<Vector2Int> connections = new List<Vector2Int>();
	public bool flat = false;

	public WorldTile(Vector2Int loc)
	{
		gridLoc = loc;
		verts = HexGrid.GetVertLocs(loc);
		tris = new List<int>();
		heights = new List<int> { 0, 0, 0, 0, 0, 0 };
	}
	public void CalcTopo()
	{
		for (int i = 0; i < verts.Count; i++)
		{
			verts[i] = new Vector3(verts[i].x, heights[i] * HexGrid.tileHeight, verts[i].z);
		}
		//find lowest height
		int level = 1000;
		foreach (int i in heights)
		{
			if (i < level)
				level = i;
		}

		//move all heights down so the lowest is 0
		for (int i = 0; i < heights.Count; i++)
		{
			heights[i] -= level;
		}

		//list of distances between opposing verts
		dis = new List<float>
		{
			(float)System.Math.Round(Vector3.Distance(verts[0], verts[3]), 3),
			(float)System.Math.Round(Vector3.Distance(verts[1], verts[4]), 3),
			(float)System.Math.Round(Vector3.Distance(verts[2], verts[5]), 3)
		};
	
		//find highest height
		float highest = 0;
		foreach (float h in heights)
		{
			if (h > highest)
				highest = h;
		}

		//calc topo
		//flat
	
		if(highest == 0)
		{
			flat = true;
			tris = new List<int> { 0, 1, 2, 0, 2, 3, 0, 3, 5, 3, 4, 5 };
			if (verts[0].y > 0.25f && Random.Range(0, 2) == 0)
			{
				Vector3 loc = HexGrid.GridToWorld(gridLoc);
				loc.y = verts[0].y;
				flat = true;
			}
		}
		else if (Mathf.Approximately(dis[0], dis[1]) && Mathf.Approximately(dis[0], dis[2]))
		{
			for (int i = 0; i < 6; i++)
			{
				if (heights[i] == 0 && heights[HexGrid.MoveDirFix(i + 1)] == 0 && heights[HexGrid.MoveDirFix(i + 2)] == 0)
				{
					tris.AddRange(new List<int> { i, i + 1, i + 2, i, i + 2, i + 3, i, i + 3, i + 5, i + 3, i + 4, i + 5 });

				}
			}
		}
		else if (dis[0] < dis[1] && dis[0] < dis[2])
		{
			if (Vector3.Distance(verts[0], verts[2]) < Vector3.Distance(verts[1], verts[3]))
				tris.AddRange(new List<int> { 0, 1, 2, 0, 2, 3 });
			else
				tris.AddRange(new List<int> { 0, 1, 3, 1, 2, 3 });
			if (Vector3.Distance(verts[0], verts[4]) < Vector3.Distance(verts[3], verts[5]))
				tris.AddRange(new List<int> { 0, 3, 4, 0, 4, 5 });
			else
				tris.AddRange(new List<int> { 0, 3, 5, 3, 4, 5 });
		}
		else if (dis[1] < dis[0] && dis[1] < dis[2])
		{
			if (Vector3.Distance(verts[1], verts[3]) < Vector3.Distance(verts[2], verts[4]))
				tris.AddRange(new List<int> { 1, 2, 3, 1, 3, 4 });
			else
				tris.AddRange(new List<int> { 1, 2, 4, 2, 3, 4 });
			if (Vector3.Distance(verts[1], verts[5]) < Vector3.Distance(verts[4], verts[0]))
				tris.AddRange(new List<int> { 1, 4, 5, 1, 5, 0 });
			else
				tris.AddRange(new List<int> { 1, 4, 0, 4, 5, 0 });
		}
		else if (dis[2] < dis[0] && dis[2] < dis[1])
		{
			if (Vector3.Distance(verts[2], verts[4]) < Vector3.Distance(verts[3], verts[5]))
				tris.AddRange(new List<int> { 2, 3, 4, 2, 4, 5 });
			else
				tris.AddRange(new List<int> { 2, 3, 5, 3, 4, 5 });
			if (Vector3.Distance(verts[2], verts[0]) < Vector3.Distance(verts[5], verts[1]))
				tris.AddRange(new List<int> { 2, 5, 0, 2, 0, 1 });
			else
				tris.AddRange(new List<int> { 2, 5, 1, 5, 0, 1 });
		}
		else if (dis[0] == dis[1] && dis[2] > dis[0] && (heights[0] == heights[1] || heights[0] == heights[4]))
		{
			tris.AddRange(new List<int> { 1, 2, 3, 1, 3, 4, 1, 4, 0, 0, 4, 5 });
		}
		else if (dis[1] == dis[2] && dis[0] > dis[1] && (heights[1] == heights[2] || heights[1] == heights[5]))
		{
			tris.AddRange(new List<int> { 0, 1, 5, 5, 1, 2, 5, 2, 4, 2, 3, 4 });
		}
		else if (dis[2] == dis[0] && dis[1] > dis[2] && (heights[2] == heights[0] || heights[2] == heights[0]))
		{
			tris.AddRange(new List<int> { 0, 1, 2, 0, 2, 3, 0, 3, 5, 3, 4, 5 });
		}
		else
		{
			tris.AddRange(new List<int> { 0, 1, 2, 0, 2, 3, 0, 3, 5, 3, 4, 5 });
		}
		for (int j = 0; j < tris.Count; j++)
		{
			tris[j] = HexGrid.MoveDirFix(tris[j]);
		}
		uvs = new List<Vector2>
		{
			new Vector2(0.5f, 1),
			new Vector2(0.9375f, 0.75f),
			new Vector2(0.9375f, 0.25f),
			new Vector2(0.5f, 0),
			new Vector2(0.0625f, 0.25f),
			new Vector2(0.0625f, 0.75f)
		};
		if (tris.Count != 12)
			tris = new List<int> { 0, 1, 2, 0, 2, 3, 0, 3, 5, 3, 4, 5 };
	}
}