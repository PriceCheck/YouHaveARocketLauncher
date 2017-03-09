using UnityEngine;
using System.Collections;

public class InnerExplosion : MonoBehaviour {
    public float ExpansionRate = 5.0f;
    public float CutOff = 2.0f;
    public float DeflateRate = 3.0f;
    public float DeathTimer = 1.0f;

    enum ExplosionState { Expanding, Shrinking};
    private ExplosionState m_state = ExplosionState.Expanding;
    private 

	// Use this for initialization
	void Start () {
        StartCoroutine(DeathTimerFunc());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        switch (m_state)
        {
            case (ExplosionState.Expanding):
                {
                    Vector3 scale = transform.localScale;
                    scale *= ExpansionRate;
                    transform.localScale = scale;
                    if (scale.x > CutOff)
                    { m_state = ExplosionState.Shrinking; }
                    break;
                }
            case (ExplosionState.Shrinking):
                {
                    Vector3 scale = transform.localScale;
                    scale /= DeflateRate;
                    transform.localScale = scale;
                    break;
                }
        }
	}

    void OnCollisionEnter(Collision col)
    {

    }

    IEnumerator DeathTimerFunc()
    {
        yield return new WaitForSeconds(DeathTimer);
        Destroy(this.gameObject);
    }
}
