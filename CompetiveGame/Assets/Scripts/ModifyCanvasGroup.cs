using UnityEngine;
using System.Collections;

public class ModifyCanvasGroup : MonoBehaviour {
    public CanvasGroup input;
    public bool FadeIn = true;
    bool working = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnButtonClicked()
    {
        if(!working)
        {
            if(FadeIn)
            {
                StartCoroutine(fadeIn());
            }
            else
            {
                StartCoroutine(fadeOut());
            }
        }
    }

    IEnumerator fadeOut()
    {
        
        input.interactable = false;
        working = true;
        if(input.alpha == 0)
        {
            yield break;
        }
        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            input.alpha = timer / 0.5f;
            yield return new WaitForSeconds(0.01f);
        }
        input.alpha = 0;
        input.blocksRaycasts = false;
        working = false;
    }

    IEnumerator fadeIn()
    {
        
        input.blocksRaycasts = true;
        working = true;
        if (input.alpha == 1)
        {
            yield break;
        }
        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            input.alpha = (0.5f - timer) / 0.5f;
            yield return new WaitForSeconds(0.01f);
        }
        input.alpha = 1;
        
        input.interactable = true;
        working = false;
    }
}
