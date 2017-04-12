using UnityEngine;
using System.Collections;
using com.ootii.Messages;

public struct ExplosionData
{
    public int playerNumber;
    public Vector3 CollisionNormal;
}

public class OuterExplosion : MonoBehaviour {
    public float ExpansionRate = 1.5f;
    public float CutOff = 1.3f;
    public float SlowExpansionRate = 1.2f;
    public float DeathTimer = 1.0f;
    bool slowMode = false;
    CharacterControllerNuevo[] PlayerCharacters;

    // Use this for initialization
    void Start() {
        StartCoroutine(DeathTimerFunc());
        PlayerCharacters = FindObjectsOfType<CharacterControllerNuevo>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 scale = transform.localScale;
        if (slowMode)
        { scale *= SlowExpansionRate; }
        else
        { scale *= ExpansionRate; }
        transform.localScale = scale;
        if (scale.x > CutOff)
        {
            slowMode = true;
        }
        RaycastHit hit;
        for(int i = 0; i < PlayerCharacters.Length; ++i)
        {
            Ray ToPlayerRay = new Ray(transform.position, PlayerCharacters[i].gameObject.transform.position - transform.position);
          //  print(PlayerCharacters[i].gameObject.name);
            Debug.DrawRay(ToPlayerRay.origin, ToPlayerRay.direction * scale.x * GetComponent<SphereCollider>().radius, Color.red, 1);
            int myLayerMask = (1 << LayerMask.NameToLayer("Player1")) | (1 << LayerMask.NameToLayer("Player2"));

            if (Physics.Raycast(ToPlayerRay, out hit, scale.x * GetComponent<SphereCollider>().radius, myLayerMask))
            {
             //   print(hit.collider.gameObject.name);
                if (hit.collider.gameObject.GetComponent<CharacterControllerNuevo>() != null)
                {
                    hit.collider.gameObject.GetComponent<CharacterControllerNuevo>().OnExplosionHit(-1*hit.normal);
                }
            }
        }
        
    }



    IEnumerator DeathTimerFunc()
    {
        yield return new WaitForSeconds(DeathTimer);
        Destroy(this.gameObject);
    }

}
