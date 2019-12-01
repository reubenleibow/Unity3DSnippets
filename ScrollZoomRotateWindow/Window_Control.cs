using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Control : MonoBehaviour
{
	//Global Values
	public Camera MainCamera;
	public int ViewPortWidth = 0;
	public int ViewPortHeight = 0;
	private float dt;

	//Values used for scolling
	private int MouseDistanceFromEdgeOfScreen = 1;
	public Vector2 CurrentMousePositionInPixels;
	public Vector3 Direction_Up;
	private const int PanningSpeed = 15;

	//Values used for zooming
	private float ZoomTargetY = 50;
	private const int ZoomTransformationSpeed = 10;
	private const int ZoomYModifier = 1000;
	private const int ZoomOutMaxY = 50;
	private const int ZoomInMinY = 10;
	private const float ZoomBuffer = 1;

	//Values used for rotating
	private Vector3 StartMousePos;
	private Vector3 RotationPoint;
	private bool MiddleMouseButtonDown = false;
	private int RotaionSpeed = 20;
	private int BufferBeforeRotation = 15;
	private float LastXMoved;

	void Start()
    {
		ViewPortWidth = Camera.main.pixelWidth;
		ViewPortHeight = Camera.main.pixelHeight;
	}

	void Update()
    {
		dt = Time.deltaTime;

		ZoomWindow_Update();

		if(!MiddleMouseButtonDown)
			PanningWindow_Update();

		RotateCamera();
	}

	//This function handles the panning of the screen.
	public void PanningWindow_Update()
	{
		var DT_PanningSpeed = ((ZoomTargetY - ZoomInMinY) / (ZoomOutMaxY - ZoomInMinY) + 1) * PanningSpeed * dt;

		//this gets the 3D direction that points up on a 2D vector
		Direction_Up = Vector3.Cross(MainCamera.transform.right, Vector3.up);

		CurrentMousePositionInPixels = new Vector2(Camera.main.ScreenToViewportPoint(Input.mousePosition).x * ViewPortWidth,
			Camera.main.ScreenToViewportPoint(Input.mousePosition).y * ViewPortHeight);

		if (CurrentMousePositionInPixels.x < MouseDistanceFromEdgeOfScreen)
			MainCamera.transform.Translate(Vector2.right * -DT_PanningSpeed);

		if (CurrentMousePositionInPixels.x > ViewPortWidth - MouseDistanceFromEdgeOfScreen)
			MainCamera.transform.Translate(Vector2.right * DT_PanningSpeed);


		if (CurrentMousePositionInPixels.y < MouseDistanceFromEdgeOfScreen)
			MainCamera.transform.Translate(Direction_Up * -DT_PanningSpeed, Space.World);

		if (CurrentMousePositionInPixels.y > ViewPortHeight - MouseDistanceFromEdgeOfScreen)
			MainCamera.transform.Translate(Direction_Up * DT_PanningSpeed, Space.World);
	}

	//This function handles the zooming in and out.
	public void ZoomWindow_Update()
	{
		var CurrentDirectionZoom = Input.GetAxis("Mouse ScrollWheel");
		var ZoomCurrentY = MainCamera.transform.position.y;

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

	//This function handles the rotaion of the screen.
	public void RotateCamera()
	{
		var mouseInputPos = Input.mousePosition;

		if (Input.GetMouseButton(2))
		{
			if(!MiddleMouseButtonDown)
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseInputPos), out RaycastHit hitInfo))
				{
						StartMousePos = mouseInputPos;
						RotationPoint = hitInfo.point;
				}

				MiddleMouseButtonDown = true;
			}

			var MovedMouseDirectionX = mouseInputPos.x - StartMousePos.x;
			var rotationModifier = (LastXMoved - mouseInputPos.x);

			if (MovedMouseDirectionX > BufferBeforeRotation)
				MainCamera.transform.RotateAround(RotationPoint, Vector3.up, rotationModifier * RotaionSpeed * dt);

			if (MovedMouseDirectionX < -BufferBeforeRotation)
				MainCamera.transform.RotateAround(RotationPoint, Vector3.up, rotationModifier * RotaionSpeed * dt);
		}
		else
			MiddleMouseButtonDown = false;

		LastXMoved = mouseInputPos.x;
	}

}
