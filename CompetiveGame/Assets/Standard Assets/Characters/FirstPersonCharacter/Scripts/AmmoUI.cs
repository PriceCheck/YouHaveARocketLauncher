using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class AmmoUI : MonoBehaviour {
    bool reloading = false;
    
    public Color reloadingColor;
    public Color FullColor;
    private Image UI;
    private float timeRemaining = 0.0f;
    private float totalTime = 0.0f;
    private float StartingScale;
    private float startingPos;

	// Use this for initialization
	void Start () {
        UI = GetComponent<Image>();
        UI.color = FullColor;
        StartingScale = transform.localScale.y;
        startingPos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool ReloadAmmo(float ReloadTime)
    {
        if (reloading == false)
        {
            UI.color = reloadingColor;
            totalTime = ReloadTime;
            timeRemaining = ReloadTime;
            StartCoroutine(Animation());

            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Animation()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            //Scale the box by the time remaining
            Vector3 newScale = transform.localScale;
            newScale.y = StartingScale * ((totalTime - timeRemaining) / totalTime);
            transform.localScale = newScale;

            yield return new WaitForSeconds(0.01f);
        }
        //Reset to starting scale
        Vector3 newScale2 = transform.localScale;
        newScale2.y = StartingScale;
        transform.localScale = newScale2;

        UI.color = FullColor;
    }
}
