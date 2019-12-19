using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSpawner : MonoBehaviour
{
	public GameObject structure;

	public void SpawnStructures(List<Vector3Int> flatTiles)
	{
		WorldData wd = GetComponent<WorldData>();
		for (int i = 0; i < flatTiles.Count; i++)
		{
			Instantiate(structure, HexGrid.GridToWorld(flatTiles[i]), Quaternion.identity);
		}
	}
}
