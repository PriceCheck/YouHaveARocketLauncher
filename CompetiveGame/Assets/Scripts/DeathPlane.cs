using UnityEngine;
using System.Collections;

public class DeathPlane : MonoBehaviour {
    bool hasScored = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (hasScored) return;
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Player2")
        {
            
            hasScored = true;
            if(col.gameObject.tag == "Player")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(2);
            }
            else if(col.gameObject.tag == "Player2")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(1);
            }
            GetComponent<AudioSource>().Play();
        }
    }
}
