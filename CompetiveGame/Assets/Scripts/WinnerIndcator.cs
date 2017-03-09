using UnityEngine;
using System.Collections;

public class WinnerIndcator : MonoBehaviour {
    [HideInInspector]
    public int Winner = 0;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonDown(0))
        {
            Winner = 0;
            Destroy(gameObject);
        }
	}
}
