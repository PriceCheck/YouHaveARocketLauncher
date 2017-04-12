using UnityEngine;
using System.Collections;
using com.ootii.Messages;

public class RocketLauncher : MonoBehaviour {

    public float ReloadTime = 0.4f;
    //AmmoUI indicator;
    private bool canShoot = true;
    public GameObject prefab;
    int playerNumber = 1;
    public bool Deactived = false;

    void Awake()
    {
        //  StartCoroutine(Setup());
        MessageDispatcher.AddListener("PlayerReloaded", OnReload);
        playerNumber = this.gameObject.transform.parent.parent.gameObject.GetComponent<CharacterControllerNuevo>().CharacterID;
    }

    void OnReload(IMessage mess)
    {
        canShoot = true;
    }

   public void onShoot()
    {
        if (Deactived) return;
        if (canShoot )//&& indicator != null && indicator.gameObject.activeInHierarchy && indicator.ReloadAmmo(ReloadTime))
        {
            
            GameObject rocket = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
            rocket.GetComponent<RocketLogic>().RocketDirection = transform.forward;
            canShoot = false;

            PlayerFiredMessage data;
            data.playerNumber = playerNumber;
            data.reloadTime = ReloadTime;
            MessageDispatcher.SendMessage(this, "PlayerFired", data, 0);
        }
    }

}
