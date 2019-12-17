using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour
{
	private Vector2 MouseStart;
	private Vector2 MouseCurrent;
	private bool MouseDown = false;
	private bool IsDragSelecting = false;
	public Image SelectionBox;
	public GameObject SelectionBoundingBox;
	public GameObject Canvas;

	public static List<GameObject> All_Selectable = new List<GameObject>();
	public static List<GameObject> Selecting_Objects = new List<GameObject>();
	public static List<GameObject> Selected_Objects = new List<GameObject>();

	public static List<GameObject> Selected_Objects_BoundingBox = new List<GameObject>();


	private const float SelectionTimeDelay = 0.3f;
	private const int SelectionDistanceDelay = 20;


	void Update()
	{
		float SelectionTimeDelayCurrent = 0f;

		//Start the Selestion box
		if (Input.GetMouseButton(0))
		{
			SelectionTimeDelayCurrent += Time.deltaTime;
			MouseCurrent = Input.mousePosition;
			SelectionBox.enabled = true;

			//set the start position of a drag
			if (MouseDown == false)
			{
				MouseDown = true;
				MouseStart = Input.mousePosition;
			}

			//Determine whether it has reached the requirements to be drag selection
			if (SelectionTimeDelayCurrent >= SelectionTimeDelay || Vector2.Distance(MouseStart,MouseCurrent) > SelectionDistanceDelay)
			{
				IsDragSelecting = true;
			}

			//work out the size and position of the selection box
			float x = Mathf.Min(MouseCurrent.x, MouseStart.x);
			float y = Mathf.Min(MouseCurrent.y, MouseStart.y);
			SelectionBox.rectTransform.position = new Vector2(x, y);

			float SizeX = MouseCurrent.x - MouseStart.x;
			float SizeY = MouseCurrent.y - MouseStart.y;
			SelectionBox.rectTransform.sizeDelta = new Vector2(Mathf.Abs(SizeX), Mathf.Abs(SizeY));
		}
		else
		{
			MouseDown = false;
			SelectionTimeDelayCurrent = 0;
			SelectionBox.enabled = false;
		}

		if (!IsDragSelecting)
		{
			//Single selecting
			if (Input.GetMouseButtonUp(0))
			{
				var allData = System_Method_Container.On_Click_Location();

				if (EventSystem.current.currentSelectedGameObject?.tag != "UI")
				{
					var Object = allData.Object;
					if (All_Selectable.Contains(Object))
						Selecting_Objects.Add(Object);

					IsDragSelecting = false;
					SelectObjects();
				}
			}
		}
		else
		{
			SelectingObjects();

			if (!MouseDown)
			{
				IsDragSelecting = false;
				SelectObjects();
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			Selecting_Objects.Clear();

			if(Selected_Objects.Count == 0)
			{
				Buttons_Control_Script.HideButtons();
			}

			SelectionBox.enabled = false;
		}

		//this snaps the position of the bouding boxes to the selected objects
		var k = 0;

		foreach(var selectdeUnits in Selected_Objects.ToArray())
		{
			if(selectdeUnits.GetComponent<Selection_Attachment>().HasBoundingBox)
			{
				var screenPos = Camera.main.WorldToScreenPoint(selectdeUnits.transform.position);
				Selected_Objects_BoundingBox[k].GetComponent<Image>().rectTransform.position = screenPos;
				k++;
			}
		}
	}

	/// <summary>
	/// (Internal)Selecting Objects
	/// </summary>
	//
	private void SelectingObjects()
	{
		var X1 = MouseStart.x;
		var Y1 = MouseStart.y;
		var X2 = MouseCurrent.x;
		var Y2 = MouseCurrent.y;

		foreach (var item in All_Selectable.ToArray())
		{
			if (All_Selectable.Contains(item))
			{
				var itemPos = Camera.main.WorldToScreenPoint(item.transform.position);

				if (itemPos.x < Mathf.Max(X1, X2) &&
					itemPos.x > Mathf.Min(X1, X2) &&
					itemPos.y < Mathf.Max(Y1, Y2) &&
					itemPos.y > Mathf.Min(Y1, Y2) &&
					item.GetComponent<Selection_Attachment>().DragSelection)
				{
					if (!Selecting_Objects.Contains(item))
					{
						Selecting_Objects.Add(item);
					}
				}
				else
				{
					if (Selecting_Objects.Contains(item))
					{
						Selecting_Objects.Remove(item);
					}
				}
			}
		}
	}

	/// <summary>
	/// (Internal)This Permanently Selects object both single and drag. and fills array "Selected_Objects"
	/// </summary>
	private void SelectObjects()
	{
		foreach(var item in Selected_Objects_BoundingBox.ToArray())
		{
			Destroy(item);
		}
		Selected_Objects_BoundingBox.Clear();

		//first set all selected objects to false (to deslect objec that were originally selected)
		if (Selected_Objects.Count > 0)
		{
			foreach (var Object in Selected_Objects)
			{
				if (Selected_Objects.Contains(Object))
					Object.GetComponent<Selection_Attachment>().Selected = false;
			}
		}

		Selected_Objects = Selecting_Objects.ToList();

		if (Selected_Objects.Count == 0)
			return;

		//select the new objects
		foreach (var Object in Selected_Objects)
		{
			GameObject newBox = null;

			if (Selected_Objects.Contains(Object))
				Object.GetComponent<Selection_Attachment>().Selected = true;

			//if the unit has requires a bounding box, then create boxes, on selected
			if (Object.GetComponent<Selection_Attachment>().HasBoundingBox)
			{
				newBox = Instantiate(SelectionBoundingBox);
				newBox.transform.parent = Canvas.transform;
				Selected_Objects_BoundingBox.Add(newBox);
			}
		}
	}

	/// <summary>
	/// (Command) Remove objects properly icluding from lists
	/// </summary>
	/// <param name="Object"></param>
	public void Destroying_Objects(GameObject Object)
	{
		if (Selected_Objects.Contains(Object))
		{
			Selected_Objects.Remove(Object);
		}

		if (Selecting_Objects.Contains(Object))
		{
			Selecting_Objects.Remove(Object);
		}

		if (All_Selectable.Contains(Object))
		{
			All_Selectable.Remove(Object);
		}

		Destroy(Object);
		Buttons_Control_Script.HideButtons();
	}

	/// <summary>
	/// (Command) Selects a particular object object
	/// </summary>
	/// <param name="Object"></param>
	public void SelectThisObject(GameObject Object)
	{
		Selecting_Objects.Add(Object);
		SelectObjects();
	}
}
