using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class User_Script : MonoBehaviour
{
	public Vector2 MouseStart;
	public Vector2 MouseCurrent;
	public bool MouseDown = false;
	public bool IsDragSelecting = false;
	public Image SeletionBox;
	public GameObject test;
	public List<GameObject> Selecting_Objects = new List<GameObject>();
	public List<GameObject> All_Selectable = new List<GameObject>();
	public List<GameObject> Selected_Objects = new List<GameObject>();

	private float SelectionTimeDelay = 0.3f;
	private float SelectionTimeDelayCurrent = 0f;
	private int SelectionDistanceDelay = 20;

	public Button Delete_Rock;

	RaycastHit hitInfo;


	void Start()
	{
		All_Selectable.Add(test);
		Delete_Rock.gameObject.SetActive(false);
	}

	void Update()
	{
		//Selestion box
		if (Input.GetMouseButton(0))
		{
			SelectionTimeDelayCurrent += Time.deltaTime;
			MouseCurrent = Input.mousePosition;
			SeletionBox.enabled = true;

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
			SeletionBox.rectTransform.position = new Vector2(x, y);

			float SizeX = MouseCurrent.x - MouseStart.x;
			float SizeY = MouseCurrent.y - MouseStart.y;
			SeletionBox.rectTransform.sizeDelta = new Vector2(Mathf.Abs(SizeX), Mathf.Abs(SizeY));
		}
		else
		{
			MouseDown = false;
			SelectionTimeDelayCurrent = 0;
			SeletionBox.enabled = false;
		}


		if (!IsDragSelecting)
		{
			//Single selecting
			if (Input.GetMouseButtonUp(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (EventSystem.current.currentSelectedGameObject?.tag != "UI" && Physics.Raycast(ray, out hitInfo))
				{
					var Object = hitInfo.collider.gameObject;

					if (All_Selectable.Contains(hitInfo.collider.gameObject))
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
				HideButtons();
			}

			SeletionBox.enabled = false;
		}
	}

	//Object that are currently being selected
	public void SelectingObjects()
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
					item.GetComponent<Selection_Behaviour>().DragSelection)
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

	//Permanently select the objects.
	public void SelectObjects()
	{
		if (Selected_Objects.Count > 0)
		{
			foreach (var Object in Selected_Objects)
			{
				if (Selected_Objects.Contains(Object))
					Object.GetComponent<Selection_Behaviour>().Selected = false;
			}
		}

		Selected_Objects = Selecting_Objects.ToList();

		if (Selected_Objects.Count == 0)
			return;

		foreach (var Object in Selected_Objects)
		{
			if (Selected_Objects.Contains(Object))
				Object.GetComponent<Selection_Behaviour>().Selected = true;
		}

		if (Selected_Objects[0].transform.tag == "Rock")
			Rock_Selected(Selected_Objects[0]);
		else
			Delete_Rock.gameObject.SetActive(false);
	}


	public void Rock_Selected(GameObject Object)
	{
		Delete_Rock.gameObject.SetActive(true);
	}

	//Removing object from list if destroyed
	public void Destroying_Objects(GameObject Object)
	{
		if (Selected_Objects.Contains(Object))
		{
			Selecting_Objects.Remove(Object);
		}

		if (Selected_Objects.Contains(Object))
		{
			Selected_Objects.Remove(Object);
		}

		if (All_Selectable.Contains(Object))
		{
			All_Selectable.Remove(Object);
		}

		Destroy(Object);
		HideButtons();
	}

	public void HideButtons()
	{
		Delete_Rock.gameObject.SetActive(false);
	}
}
