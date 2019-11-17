using UnityEngine;
using System.Collections;

public class DeviceAffordances : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTapButton(Gesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            
        }

    }
    void OnFingerUp(FingerUpEvent e)
    {
        if (e.Selection == this.gameObject)
        {
            GameObject[] furnitures = GameObject.FindGameObjectsWithTag("Furniture");
            for (int i = 0; i < furnitures.Length; i++)
            {
                if (furnitures[i].transform.name.Contains("wall") || furnitures[i].transform.name.Contains("Wall") || furnitures[i].transform.name.Contains("Floor") || furnitures[i].transform.name.Contains("floor") || furnitures[i].transform.name.Contains("ciling") || furnitures[i].transform.name.Contains("Ceiling"))
                {
                    Debug.Log(furnitures[i].transform.name);
                }
                else if (furnitures[i].GetComponent<Renderer>())
                {
                    furnitures[i].GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    furnitures[i].GetComponentInChildren<Renderer>().enabled = true;
                }

            }
        }
        
    }
    void OnFingerDown(FingerDownEvent e)
    {
        if (e.Selection == this.gameObject)
        {
            GameObject[] furnitures = GameObject.FindGameObjectsWithTag("Furniture");
            for (int i = 0; i < furnitures.Length; i++)
            {
                if (furnitures[i].transform.name.Contains("wall") || furnitures[i].transform.name.Contains("Wall") || furnitures[i].transform.name.Contains("Floor") || furnitures[i].transform.name.Contains("floor") || furnitures[i].transform.name.Contains("ciling") || furnitures[i].transform.name.Contains("Ceiling"))
                {
                    Debug.Log(furnitures[i].transform.name);
                }
                else if (furnitures[i].GetComponent<Renderer>())
                {
                    furnitures[i].GetComponent<Renderer>().enabled = false;
                }
                else
                {
                    furnitures[i].GetComponentInChildren<Renderer>().enabled = false;
                }

            }
        }
       
    }
}
