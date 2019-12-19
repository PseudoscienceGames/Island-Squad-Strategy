using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(WorldMesh))]
public class WorldData : MonoBehaviour
{
	public static WorldData Instance;
	private void Awake(){ Instance = this; }

	public int worldSize;
	public List<Vector3Int> islandInfo = new List<Vector3Int>();
	public List<IslandData> islands = new List<IslandData>();
	private List<Vector2Int> usedTiles = new List<Vector2Int>();
	public List<WorldTile> worldTiles = new List<WorldTile>();

	private void Start()
	{
		GenWorldData();
		List<Vector3Int> flatTiles = new List<Vector3Int>();
		foreach(IslandData id in islands)
		{
			foreach(WorldTile wt in id.tiles)
			{
				if (wt.flat)
					flatTiles.Add(new Vector3Int(wt.gridLoc.x, wt.gridLoc.y, Mathf.RoundToInt(wt.verts[0].y / HexGrid.tileHeight)));
			}
		}
		GetComponent<StructureSpawner>().SpawnStructures(flatTiles);
	}

	void GenWorldData()
	{
		
		foreach(Vector3Int v in islandInfo)
		{
			for (int i = 0; i < v.z; i++)
			{
				AddIsland(Random.Range(v.x, v.y));
			}
		}
		
		Debug.Log(usedTiles.Count);
		GetComponent<WorldMesh>().GenMesh();
	}

	void AddIsland(int islandSize)
	{
		if (islandSize < 2) islandSize = 2;
		List<Vector2Int> tiles = new List<Vector2Int>();
		List<Vector2Int> possTiles = new List<Vector2Int>();

		IslandData currentIsland = new IslandData();
		Vector2Int loc = new Vector2Int(Random.Range(-worldSize, worldSize), Random.Range(-worldSize, worldSize));
		int x = 0;
		while (usedTiles.Contains(loc))
		{
			if (x >= 10)
			{
				worldSize++;
				x = 0;
				Debug.Log("X");
			}
			Vector3 spot = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * worldSize;
			loc = HexGrid.RoundToGrid(spot);
			x++;
		}
		tiles.Add(loc);

		foreach (Vector2Int v in HexGrid.FindAdjacentGridLocs(loc))
		{
			if (!tiles.Contains(v) && !possTiles.Contains(v) && !usedTiles.Contains(v))
				possTiles.Add(v);
		}
		while (tiles.Count < islandSize && possTiles.Count > 0)
		{
			Vector2Int tile = possTiles[Random.Range(0, possTiles.Count)];
			foreach (Vector2Int v in HexGrid.FindAdjacentGridLocs(tile))
			{
				if (!tiles.Contains(v) && !possTiles.Contains(v) && !usedTiles.Contains(v))
					possTiles.Add(v);
			}
			possTiles.Remove(tile);
			tiles.Add(tile);
		}
        if (tiles.Count == islandSize)
        {
            foreach (Vector2Int v in HexGrid.FindOutline(tiles))
                tiles.Add(v);
            List<Vector2Int> remove = new List<Vector2Int>();
            remove.AddRange(tiles);
            remove.AddRange(HexGrid.FindOutline(remove));
            remove.AddRange(HexGrid.FindOutline(remove));
            remove.AddRange(HexGrid.FindOutline(remove));

            foreach (Vector2Int v in remove)
                usedTiles.Add(v);
            foreach (Vector2Int v in tiles)
            {
                currentIsland.tiles.Add(new WorldTile(v));
                currentIsland.gridLocs.Add(v);
            }
            foreach (Vector2Int v in tiles)
            {
                foreach (Vector2Int adj in HexGrid.FindAdjacentGridLocs(v))
                {
                    if (tiles.Contains(adj))
                        currentIsland.tiles[currentIsland.gridLocs.IndexOf(v)].connections.Add(adj);
                }
            }
            currentIsland.CalcHeights();
            islands.Add(currentIsland);
        }
        else
            AddIsland(islandSize);
	}

	public void Mark(Vector3 loc)
	{
		GameObject tree = Instantiate(Resources.Load("Marker"), loc, transform.rotation) as GameObject;
		//tree.transform.localScale = Vector3.one * Random.Range(0.4f, 0.9f) + Vector3.up * Random.Range(-0.2f, 0.5f);
		tree.transform.Rotate(Vector3.up * (Random.Range(0, 6) * 60));
	}
}
