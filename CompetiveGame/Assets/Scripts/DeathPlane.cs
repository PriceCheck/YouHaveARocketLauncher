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
        print("Hit");
        if (hasScored) return;
        if(col.gameObject.tag == "Player")
        {
            
            hasScored = true;
            if(col.gameObject.name == "Player1")
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
    void OnControllerColliderHit(ControllerColliderHit col)
    {
        print("Hit");
        if (hasScored) return;
        if (col.gameObject.tag == "Player")
        {

            hasScored = true;
            if (col.gameObject.name == "Player1")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(2);
            }
            else if (col.gameObject.tag == "Player2")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(1);
            }
            GetComponent<AudioSource>().Play();
        }
    }

    public void CollisionResolution(GameObject gam)
    {
        if (hasScored) return;
        if (gam.tag == "Player")
        {
            hasScored = true;
            if (gam.name == "Player1")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(2);
            }
            else if (gam.tag == "Player2")
            {
                GameObject.FindObjectOfType<GameplayManager>().HasScored(1);
            }
        //    GetComponent<AudioSource>().Play();
        }
    }
}
