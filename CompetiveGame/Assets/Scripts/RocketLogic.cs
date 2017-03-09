using UnityEngine;
using System.Collections;

public class RocketLogic : MonoBehaviour {
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public GameObject explosionSound;
    public bool isDead = false;
	// Use this for initialization
	void Start () {
        StartCoroutine(DestoryAfterTimer());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (!isDead)
        {
            Instantiate(explosionPrefab, col.contacts[0].point, transform.rotation);
            Instantiate(explosionPrefab2, col.contacts[0].point, transform.rotation);
            Instantiate(explosionSound, col.contacts[0].point, transform.rotation);
            Destroy(this.gameObject);
            isDead = true;
        }
    }

    IEnumerator DestoryAfterTimer()
    {
        yield return new WaitForSeconds(2.0f);
        
    }
}
