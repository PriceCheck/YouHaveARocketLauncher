using UnityEngine;
using System.Collections;

public class RocketLogic : MonoBehaviour {
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public GameObject explosionSound;
    [HideInInspector]
    public Vector3 RocketDirection;
    float RocketVelcoityScale = 50;
    AnimationCurve SpeedScale = AnimationCurve.EaseInOut(0, 0.7f, 1, 1);
    public bool isDead = false;
    float timer = 0;
    float totalTime = 4f;
	// Use this for initialization
	void Start () {
        StartCoroutine(DestoryAfterTimer());
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        GetComponent<Rigidbody>().velocity = RocketDirection * (RocketVelcoityScale * SpeedScale.Evaluate(timer / totalTime));
	}

    void OnCollisionEnter(Collision col)
    {
        if (!isDead && timer > 0.01f)
        {
            Instantiate(explosionPrefab, col.contacts[0].point, transform.rotation);
            Instantiate(explosionPrefab2, col.contacts[0].point, transform.rotation);
            Instantiate(explosionSound, col.contacts[0].point, transform.rotation);
            DestroyRocket();
        }
    }

    void DestroyRocket()
    {

        Destroy(this.gameObject);
        isDead = true;
    }

    IEnumerator DestoryAfterTimer()
    {
        yield return new WaitForSeconds(totalTime);
        DestroyRocket();

    }
}
