using UnityEngine;
using System.Collections;

public class DestructableCube : MonoBehaviour {

    private float timeRemaining = 0;
    private float deathAnimationTime = 0.25f;
    private Vector3 startingScale;
	// Use this for initialization
	void Start () {
        startingScale = transform.localScale;
	}

    void OnCollisionEnter(Collision col)
    {
       // print("Look ma " + col.gameObject.name + " " + col.gameObject.tag);
        if(col.gameObject.tag == "Boom")
        {
            timeRemaining = deathAnimationTime;
            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation()
    {
        while (timeRemaining > 0)
        {
            
            //Scale the box by the time remaining
            Vector3 newScale = transform.localScale;
            newScale = startingScale * (timeRemaining / deathAnimationTime);
            transform.localScale = newScale;

            timeRemaining -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        //Reset to starting scale
        Destroy(gameObject);
    }
}
