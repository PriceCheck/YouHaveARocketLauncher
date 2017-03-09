using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinnerText : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        WinnerIndcator obj = GameObject.FindObjectOfType<WinnerIndcator>();
        Text tex = GetComponent<Text>();
        if(obj != null && obj.Winner == 1)
        {
            tex.text = "Red player wins";
        }
        else
        {
            tex.text = "Blue player wins";
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
