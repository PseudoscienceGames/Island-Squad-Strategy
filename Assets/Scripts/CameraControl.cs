using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
	public float zoom;
	public float targetZoom;
	public int zoomMin;
	public int zoomMax;
	public float zoomSpeed;
	public float cameraRotSpeed;
	public float camPanSpeed;
	public int scrollAreaSize;
	public bool screenEdgeScroll;

	public GameObject cameraPivot;

	public LayerMask mask;
	public Vector3 oldPos;

	public float smoothTime;
	public float rotSmoothTime;
	public float camRotSmoothTime;
	public float zoomSmoothTime;
	private  Vector3 velocity = Vector3.zero;
	private Vector3 rVelocity = Vector3.zero;
	private Vector3 cVelocity = Vector3.zero;
	private Vector3 zVelocity = Vector3.zero;

	public float camXRotMin;
	public float camXRotMax;

	public static CameraControl Instance;
	void Awake() { Instance = this; }

	private void Start()
	{
		Invoke("Raise", 0.2f);
	}
	void Raise()
	{
		if(Physics.Raycast(transform.position + (Vector3.up * 50), -Vector3.up, out RaycastHit hit, 500, mask))
			transform.position = hit.point;
	}

	void Update()
	{
		if (Input.GetAxis("Vertical") != 0)
			transform.position += transform.forward * Input.GetAxisRaw("Vertical") * camPanSpeed;
		if (Input.GetAxis("Horizontal") != 0)
			transform.position += transform.right * Input.GetAxisRaw("Horizontal") * camPanSpeed;
		//Pivots camera based on mouse movement
		if (Input.GetMouseButton(1))
		{
			//transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * cameraRotSpeed * Time.deltaTime, Space.Self);
			Vector3 targetEuler = transform.eulerAngles + (Vector3.up * Input.GetAxis("Mouse X") * cameraRotSpeed);
			transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, targetEuler, ref rVelocity, rotSmoothTime);
			//cameraPivot.transform.Rotate(-Vector3.right, Input.GetAxis("Mouse Y") * cameraRotSpeed * Time.deltaTime);
			Vector3 camTargetEuler = cameraPivot.transform.eulerAngles + (-Vector3.right * Input.GetAxis("Mouse Y") * cameraRotSpeed);
			camTargetEuler = Vector3.SmoothDamp(cameraPivot.transform.eulerAngles, camTargetEuler, ref cVelocity, camRotSmoothTime);
			camTargetEuler.x = Mathf.Clamp(camTargetEuler.x, camXRotMin, camXRotMax);
			cameraPivot.transform.eulerAngles = camTargetEuler;
		}
		else
		{
			//Zoom camera
			zoom = -Camera.main.transform.localPosition.z;
			if (zoom > zoomMin && zoom < zoomMax)
			{
					targetZoom += -(Input.GetAxisRaw("Mouse ScrollWheel")) * zoomSpeed * (zoom / zoomMax);
					if (targetZoom > zoomMax)
						targetZoom = zoomMax - 1;
					if (targetZoom < zoomMin)
						targetZoom = zoomMin + 1;
					Vector3 zoomV = new Vector3(0, 0, -targetZoom);
					Camera.main.transform.localPosition = Vector3.SmoothDamp(Camera.main.transform.localPosition, zoomV, ref zVelocity, zoomSmoothTime);
			}
		}
		if (Input.GetMouseButtonDown(2))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, mask))
				oldPos = hit.point;
		}
		if (Input.GetMouseButtonUp(2))
		{
			oldPos = transform.position;
			velocity = Vector3.zero;
		}
		if (transform.position.x > 100)
			transform.position = new Vector3(100, transform.position.y, transform.position.z);
		if (transform.position.z > 100)
			transform.position = new Vector3(transform.position.x, transform.position.y, 100);
		if (transform.position.x < -100)
			transform.position = new Vector3(-100, transform.position.y, transform.position.z);
		if (transform.position.z < -100)
			transform.position = new Vector3(transform.position.x, transform.position.y, -100);
	}

	private void LateUpdate()
	{
		if (Input.GetMouseButton(2))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, mask))
			{
				Vector3 targetPosition = transform.position + (oldPos - hit.point);
				if (Physics.Raycast(targetPosition + (Vector3.up * 50), -Vector3.up, out hit, 500, mask))
				{
					targetPosition = hit.point;
				}
				transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
			}
		}
	}

	public void FocusCam(Vector2Int loc)
	{
		transform.position = HexGrid.GridToWorld(loc);
	}
}