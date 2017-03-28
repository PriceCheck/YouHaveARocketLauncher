using UnityEngine;
using System.Collections;
using Text = UnityEngine.UI.Text;
using Controller = UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

public class Countdown : MonoBehaviour {
    private Text UI;
    public AudioSource countdownNormal;
    public AudioSource countdownEnd;
    public Controller player1;
    public Controller player2;
    public RocketLauncher RocketLauncher1;
    public RocketLauncher RocketLauncher2;
    Vector3 startingScale;
    public float ScaleRatio = 1.25f;
	// Use this for initialization
	void Start () {
        startingScale = transform.localScale;
        UI = GetComponent<Text>();
      //  StartCoroutine(AnimationStart());
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator AnimationStart()
    {
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        
        int i = 3;
        while (i > 0)
        {
            UI.text = "" + i;
            float timer = 1;
            countdownNormal.Play();
            while (timer > 0)
            {
               
                timer -= Time.deltaTime;
                transform.localScale = startingScale + startingScale * (ScaleRatio * timer);
                yield return new WaitForSeconds(0.01f);
            }
            --i;
            
        }
        UI.text = "FIGHT";
        countdownEnd.Play();

        player1.Deactivated = false;
        player2.Deactivated = false;
        RocketLauncher1.Deactived = false;
        RocketLauncher2.Deactived = false;

        yield return new WaitForSeconds(0.25f);

        float timer2 = 1;
        while (timer2 > 0)
        {
            timer2 -= Time.deltaTime;
            transform.localScale = startingScale * timer2;
            yield return new WaitForSeconds(0.01f);
        }

        
        Destroy(gameObject);
    }
}
