using UnityEngine;
using System.Collections;

public class SplashLogic : MonoBehaviour {
    public CanvasGroup Menu;
    public SplashLogic NextSplash;
    public float TotalTimer = 3.0f;

    public float FadeTimeOut = 0.25f;
    public float FadeTimeIn = 0.1f;
    public bool StartVisable = true;
    private CanvasGroup thisObject;
    private float fadeInTimer = 0.0f;

    float CurrentTimer; 

	// Use this for initialization
	void Start () {
        CurrentTimer = TotalTimer;
        fadeInTimer = FadeTimeIn;

        thisObject = GetComponent<CanvasGroup>();
     }
	
	// Update is called once per frame
	void Update () {
        if (StartVisable)
        {
            StartVisable = false;
            OnAwake();
        }
        if(Input.GetMouseButtonDown(0))
        {
            OnActivate();
        }

    }

    void OnAwake()
    {
        StartCoroutine(Animation());
    }

    void OnActivate()
    {
        if (Menu != null)
        {
            Menu.alpha = 1;
        }
        if(NextSplash != null)
        {
            NextSplash.OnAwake();
        }
        Destroy(gameObject);
    }

    IEnumerator Animation()
    {
        while (CurrentTimer > 0)
        {
            if(fadeInTimer > 0 )
            {
                fadeInTimer -= Time.deltaTime;
                thisObject.alpha = (FadeTimeIn - fadeInTimer) / FadeTimeIn;
            }

            if(CurrentTimer <= FadeTimeOut)
            {
                 thisObject.alpha = CurrentTimer / FadeTimeOut;
            }
            CurrentTimer -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        //Reset to starting scale
        OnActivate();
    }
}
