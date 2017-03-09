using UnityEngine;
using System.Collections;

public class RocketLauncher : MonoBehaviour {

    public float RocketSpeed = 20;
    public float ReloadTime = 1.0f;
    AmmoUI indicator;
    private bool canShoot = true;
    public GameObject prefab;
    public int playerNumber = 1;
    public bool Deactived = true;

    void Start()
    {
        StartCoroutine(Setup());
    }

   public void onShoot()
    {
        if (Deactived) return;
        if (canShoot && indicator != null && indicator.gameObject.activeInHierarchy && indicator.ReloadAmmo(ReloadTime))
        {
            GameObject rocket = (GameObject)Instantiate(prefab, transform.position + (transform.forward * 2), transform.rotation);
            rocket.GetComponent<Rigidbody>().velocity = transform.forward * RocketSpeed;
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(0.001f);
       indicator = GameObject.FindObjectOfType<GameplayManager>().GetAmmoUI(playerNumber);

    }


        IEnumerator Cooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        canShoot = true;
    }
}
