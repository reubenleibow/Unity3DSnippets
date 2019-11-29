using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class User_Script : MonoBehaviour
{
	public Vector2 MouseStart;
	public Vector2 MouseCurrent;
	public bool MouseDown = false;
	public bool IsSelecting = false;
	public Image SeletionBox;
	public GameObject test;
	public List<GameObject> Selecting_Objects = new List<GameObject>();
	public List<GameObject> All_Selectable = new List<GameObject>();
	public List<GameObject> Selected_Objects = new List<GameObject>();
	private float SelectionTimeDelay = 0.1f;
	private float SelectionTimeDelayCurrent = 0f;

	public Button Delete_Rock;

	RaycastHit hitInfo;


	void Start()
	{
		All_Selectable.Add(test);

		Delete_Rock.enabled = false;
	}

	void Update()
	{
		//Selestion box
		if (Input.GetMouseButton(0))
		{
			//Selected_Objects.Clear();
			SelectionTimeDelayCurrent += Time.deltaTime;

			if (MouseDown == false)
			{
				MouseDown = true;
				MouseStart = Input.mousePosition;
			}

			if (SelectionTimeDelayCurrent >= SelectionTimeDelay)
			{
				IsSelecting = true;
			}

			MouseCurrent = Input.mousePosition;
			SeletionBox.enabled = true;

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
		}

		if (!IsSelecting)
		{
			//Single selecting
			if (Input.GetMouseButtonUp(0))
			{
				Selected_Objects.Clear();
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hitInfo))
				{
					var Object = hitInfo.collider.gameObject;

					if (All_Selectable.Contains(hitInfo.collider.gameObject))
					{
						Selecting_Objects.Clear();
						Selecting_Objects.Add(Object);
						SelectObjects();
					}
				}
			}
		}
		else
		{
			SelectingObjects();

			if (!MouseDown)
			{
				IsSelecting = false;

				if (Selecting_Objects.Count() > 0)
				{
					SelectObjects();
				}

				SeletionBox.enabled = false;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			Selecting_Objects.Clear();
		}
	}


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

	public void SelectObjects()
	{
		Selected_Objects = Selecting_Objects.ToList();

		if (Selected_Objects[0].transform.tag == "Rock")
		{
			Destroying_Objects(Selected_Objects[0]);
		}
	}

	public void Rock_Selected()
	{

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

	}
}
