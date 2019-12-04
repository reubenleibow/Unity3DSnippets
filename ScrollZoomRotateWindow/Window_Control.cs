using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Control : MonoBehaviour
{
	//Main properties
	public GameObject ParentObject;
	public Camera MainCamera;

	private float dt;
	public int ViewPortWidth = 0;
	public int ViewPortHeight = 0;

	public float AreaWidth;
	public float AreaLegnth;
	public Terrain GameArea;

	//Scolling Properties
	private int MouseDistanceFromEdgeOfScreen = 10;
	public Vector2 CurrentMousePositionInPixels;
	private const int PanningSpeed = 15;

	//Zoom Propperies
	private float ZoomTargetY = 50;
	private const int ZoomTransformationSpeed = 20;
	//where the zoom should be,set the intensity of a scroll in a mouse
	private const int ZoomYModifier = 1000;
	private const int ZoomOutMaxY = 80;
	private const int ZoomInMinY = 5;
	//Prevents camera flickering as it nears the value
	private const float ZoomBuffer = 1;

	//Rotation Properties
	public Vector2 StartMousePos;
	public Vector3 LastDirection;
	public bool MiddleMouseButtonDown = false;
	private int RotaionSpeed = 200;

	private int MaxVerticalRotation = 80;
	private int MinVerticalRotation = 10;


	// Start is called before the first frame update
	void Start()
	{
		ViewPortWidth = Camera.main.pixelWidth;
		ViewPortHeight = Camera.main.pixelHeight;

		AreaWidth = GameArea.terrainData.size.x;
		AreaLegnth = GameArea.terrainData.size.z;
	}

	// Update is called once per frame
	void Update()
	{
		dt = Time.deltaTime;

		if (!MiddleMouseButtonDown)
		{
			PanningWindow_Update();
			ZoomWindow_Update();
		}

		ScrollWithKeys();
		RotateCamera();
	}

	public void PanningWindow_Update()
	{
		var forward = ParentObject.transform.forward;
		var right = ParentObject.transform.right;
		var dir_Up = Vector3.Cross(MainCamera.transform.right, Vector3.up);

		CurrentMousePositionInPixels = new Vector2(
			Camera.main.ScreenToViewportPoint(Input.mousePosition).x * ViewPortWidth,
			Camera.main.ScreenToViewportPoint(Input.mousePosition).y * ViewPortHeight);

		if (CurrentMousePositionInPixels.x < MouseDistanceFromEdgeOfScreen)
			ParentObject.transform.Translate(right * -PanningSpeed * dt, Space.World);

		if (CurrentMousePositionInPixels.x > ViewPortWidth - MouseDistanceFromEdgeOfScreen)
			ParentObject.transform.Translate(right * PanningSpeed * dt, Space.World);

		if (CurrentMousePositionInPixels.y < MouseDistanceFromEdgeOfScreen)
			ParentObject.transform.Translate(dir_Up * -PanningSpeed * dt, Space.World);

		if (CurrentMousePositionInPixels.y > ViewPortHeight - MouseDistanceFromEdgeOfScreen)
			ParentObject.transform.Translate(dir_Up * PanningSpeed * dt, Space.World);

		var ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.z > AreaLegnth)
			ParentObject.transform.position = new Vector3(ParentPosition.x, ParentPosition.y, AreaLegnth);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.z < 0)
			ParentObject.transform.position = new Vector3(ParentPosition.x, ParentPosition.y, 0);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.x > AreaWidth)
			ParentObject.transform.position = new Vector3(AreaWidth, ParentPosition.y, ParentPosition.z);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.x < 0)
			ParentObject.transform.position = new Vector3(0, ParentPosition.y, ParentPosition.z);
	}

	public void ZoomWindow_Update()
	{
		var CurrentDirectionZoom = -Input.GetAxis("Mouse ScrollWheel");
		var ZoomCurrentY = Vector3.Distance(MainCamera.transform.position, ParentObject.transform.position);

		ZoomTargetY += CurrentDirectionZoom * ZoomYModifier * dt;

		if (ZoomTargetY > ZoomOutMaxY)
			ZoomTargetY = ZoomOutMaxY;

		if (ZoomTargetY < ZoomInMinY)
			ZoomTargetY = ZoomInMinY;

		var zoomDistanceFromTarget = Mathf.Abs(ZoomTargetY - ZoomCurrentY);

		if (zoomDistanceFromTarget < ZoomBuffer)
			ZoomCurrentY = ZoomTargetY;

		//zooming in
		if (ZoomCurrentY < ZoomTargetY)
			MainCamera.transform.Translate(Vector3.forward * -zoomDistanceFromTarget * ZoomTransformationSpeed * dt);

		//zooming out
		if (ZoomCurrentY > ZoomTargetY)
			MainCamera.transform.Translate(Vector3.forward * zoomDistanceFromTarget * ZoomTransformationSpeed * dt);
	}

	public void RotateCamera()
	{
		var mouseInputPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

		if (Input.GetMouseButton(2))
		{
			if (!MiddleMouseButtonDown)
			{
				LastDirection = ParentObject.transform.eulerAngles;
				StartMousePos = mouseInputPos;
				MiddleMouseButtonDown = true;
			}

			var MovedMouseDirectionX = mouseInputPos.x - StartMousePos.x;
			var MovedMouseDirectionY = mouseInputPos.y - StartMousePos.y;

			var RotationVector = LastDirection + new Vector3(-MovedMouseDirectionY * RotaionSpeed, MovedMouseDirectionX * RotaionSpeed, 0);

			//set vertical constraints
			if (RotationVector.x > MaxVerticalRotation)
				RotationVector.x = MaxVerticalRotation;

			if (RotationVector.x < MinVerticalRotation)
				RotationVector.x = MinVerticalRotation;

			ParentObject.transform.eulerAngles = RotationVector;
		}
		else
			MiddleMouseButtonDown = false;

	}

	public void ScrollWithKeys()
	{
		var forward = ParentObject.transform.forward;
		var right = ParentObject.transform.right;
		var dir_Up = Vector3.Cross(MainCamera.transform.right, Vector3.up);

		if (Input.GetKey("a"))
			ParentObject.transform.Translate(right * -PanningSpeed * dt, Space.World);

		if (Input.GetKey("d"))
			ParentObject.transform.Translate(right * PanningSpeed * dt, Space.World);

		if (Input.GetKey("s"))
			ParentObject.transform.Translate(dir_Up * -PanningSpeed * dt, Space.World);

		if (Input.GetKey("w"))
			ParentObject.transform.Translate(dir_Up * PanningSpeed * dt, Space.World);

		var ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.z > AreaLegnth)
			ParentObject.transform.position = new Vector3(ParentPosition.x, ParentPosition.y, AreaLegnth);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.z < 0)
			ParentObject.transform.position = new Vector3(ParentPosition.x, ParentPosition.y, 0);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.x > AreaWidth)
			ParentObject.transform.position = new Vector3(AreaWidth, ParentPosition.y, ParentPosition.z);

		ParentPosition = ParentObject.transform.position;

		if (ParentObject.transform.position.x < 0)
			ParentObject.transform.position = new Vector3(0, ParentPosition.y, ParentPosition.z);
	}
}
