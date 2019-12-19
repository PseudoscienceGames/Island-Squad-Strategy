using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCursor : MonoBehaviour
{
	public Vector2Int gridLoc;
	public LayerMask mask;
	public int tool = 0;
	public bool overUI = false;

	public static HexCursor Instance;
	private void Awake(){ Instance = this; }

	void Update()
    {
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, mask))
		{
			gridLoc = HexGrid.RoundToGrid(hit.point);
			Vector3 pos = HexGrid.GridToWorld(gridLoc);
			pos.y = Mathf.Round(hit.point.y / HexGrid.tileHeight) * HexGrid.tileHeight;
			transform.position = pos;


			if (Input.GetMouseButtonDown(0) && !overUI)
			{
				if (tool == 1)
				{
					Instantiate(Resources.Load("Dock"), pos, Quaternion.identity);
				}
				if(tool == 2)
				{
					Instantiate(Resources.Load("Tavern"), pos, Quaternion.identity);
				}
				if (tool == 3)
				{
					Instantiate(Resources.Load("Tents"), pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0));
				}
			}
			if (Input.GetMouseButtonDown(1))
				tool = 0;
		}
	}
	public void SetTool(int i)
	{
		tool = i;
	}
	public void SetOverUI(bool o)
	{
		overUI = o;
	}
}
