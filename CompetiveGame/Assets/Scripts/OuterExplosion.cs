using UnityEngine;
using System.Collections;

public class OuterExplosion : MonoBehaviour {
    public float ExpansionRate = 1.5f;
    public float CutOff = 1.3f;
    public float SlowExpansionRate = 1.2f;
    public float DeathTimer = 1.0f;
    bool slowMode = false;

    // Use this for initialization
    void Start() {
        StartCoroutine(DeathTimerFunc());
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
    }

    void OnCollisionEnter(Collision col)
    {
        if ( (col.gameObject.tag == "Player" || col.gameObject.tag == "Player2") && col.contacts.Length > 0)
        {
            //Bounce
            var direction = col.contacts[0].point - this.gameObject.transform.position;
            direction.Normalize();
         //   col.gameObject.BroadcastMessage("Bounce", direction, SendMessageOptions.DontRequireReceiver);
        }
    }

    IEnumerator DeathTimerFunc()
    {
        yield return new WaitForSeconds(DeathTimer);
        Destroy(this.gameObject);
    }

}
