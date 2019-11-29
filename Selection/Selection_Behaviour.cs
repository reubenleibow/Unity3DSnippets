using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection_Behaviour : MonoBehaviour
{
	public bool DragSelection = true;
	public bool Selected = false;
	public Color Initial;
	public Color AlternativeColour;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Selected)
		{
			if (tag == "Rock")
				GetComponent<Renderer>().material.color = AlternativeColour;
		}
		else
		{
			if (tag == "Rock")
				GetComponent<Renderer>().material.color = Initial;
		}
    }
}
