using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IslandData
{
	public List<Vector2Int> gridLocs = new List<Vector2Int>();
	public List<WorldTile> tiles = new List<WorldTile>();
	Dictionary<Vector2Int, List<Vector2Int>> connections = new Dictionary<Vector2Int, List<Vector2Int>>();
	List<List<Vector2Int>> blobs = new List<List<Vector2Int>>();
	public Dictionary<Vector2Int, int> heights = new Dictionary<Vector2Int, int>();
	public int blobCount;

	void BuildConnections()
	{
		foreach (WorldTile w in tiles)
		{
			foreach (Vector3 v in w.verts)
			{
				Vector2Int k = new Vector2Int(Mathf.RoundToInt(v.x * 10f), Mathf.RoundToInt(v.z * 10f));
				if (!connections.ContainsKey(k))
				{
					connections.Add(k, new List<Vector2Int>());
				}
			}
		}
		foreach (WorldTile w in tiles)
		{
			foreach(Vector3 v in w.verts)
			{
				Vector2Int k = new Vector2Int(Mathf.RoundToInt(v.x * 10f), Mathf.RoundToInt(v.z * 10f));
				for (int i = 0; i < 6; i++)
				{
					Vector3 dir = Quaternion.Euler(0, i * 60f, 0) * (Vector3.forward * 0.57735f);
					Vector2Int k2 = new Vector2Int(Mathf.RoundToInt((v.x + dir.x) * 10f), Mathf.RoundToInt((v.z + dir.z) * 10f));
					if(connections.ContainsKey(k2) && !connections[k].Contains(k2))
						connections[k].Add(k2);
				}
			}
		}
	}
	void FindEdge()
	{
		List<Vector2Int> blob = new List<Vector2Int>();
		Dictionary<Vector2Int, int> vtc = new Dictionary<Vector2Int, int>();
		foreach(WorldTile w in tiles)
		{
			foreach(Vector3 v in w.verts)
			{
				Vector2Int k = new Vector2Int(Mathf.RoundToInt(v.x * 10f), Mathf.RoundToInt(v.z * 10f));
				if (!vtc.ContainsKey(k))
					vtc.Add(k, 1);
				else
					vtc[k]++;
			}
		}
		foreach(KeyValuePair<Vector2Int, int> kvp in vtc)
		{
			if (kvp.Value < 3)
				blob.Add(kvp.Key);
		}
		blobs.Add(blob);
	}
	void FindBlobs()
	{
		List<Vector2Int> left = new List<Vector2Int>();
		foreach (Vector2Int v in connections.Keys)
		{
			if (!blobs[0].Contains(v) && !left.Contains(v))
				left.Add(v);
		}
		if (left.Count > 0)
		{
			blobCount = left.Count / 10;
			if (blobCount == 0)
				blobCount = 1;
			for (int i = 0; i < blobCount; i++)
			{
				List<Vector2Int> blob = new List<Vector2Int>();
				List<Vector2Int> pot = new List<Vector2Int>();
				while (pot.Count < 4)
				{
					pot.Clear();
					Vector2Int loc = left[Random.Range(0, left.Count)];
					pot.Add(loc);
					foreach (Vector2Int v in connections[loc])
					{
						if (left.Contains(v))
							pot.Add(v);
					}
				}
				foreach (Vector2Int v in pot)
				{
					blob.Add(v);
					left.Remove(v);
				}
				blobs.Add(blob);
			}
			foreach(Vector2Int v in left)
			{
				int closest = 1;
				for (int i = 2; i < blobs.Count; i++)
				{
					if (Vector2.Distance(blobs[i][0], v) < Vector2.Distance(blobs[closest][0], v))
						closest = i;
				}
				blobs[closest].Add(v);
			}
		}
	}
	public void CalcHeights()
	{
		BuildConnections();
		FindEdge();
		FindBlobs();
		//Debug.Log(connections.Count + " " + blobs[0].Count + "  " + blobs[1].Count);
		int h = 0;
		for (int i = 0; i < blobs.Count; i++)
		{
			
			if (h == 4)
				h = 1;
			foreach (Vector2Int v in blobs[i])
			{
				if (i == 0)
					heights.Add(v, 0);
				else
					heights.Add(v, h);
			}
			h++;
		}
		foreach (WorldTile w in tiles)
		{
			for (int i = 0; i < w.heights.Count; i++)
			{
				if (heights.ContainsKey(new Vector2Int(Mathf.RoundToInt(w.verts[i].x * 10f), Mathf.RoundToInt(w.verts[i].z * 10f))))
					w.heights[i] = heights[new Vector2Int(Mathf.RoundToInt(w.verts[i].x * 10f), Mathf.RoundToInt(w.verts[i].z * 10f))];
				else
					Debug.Log("WRONG " + w.verts[i]);
			}
			w.CalcTopo();
		}
	}
}
