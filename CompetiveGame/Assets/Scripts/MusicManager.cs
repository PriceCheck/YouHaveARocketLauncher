using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    public AudioSource[] musicTracks;
    public float[] maxVolume;
	// Use this for initialization
	void Start () {
        //If a music manager already exists, don't exist
	    if(GameObject.FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FadeOutTrack(int index)
    {
       // if(index < musicTracks.Length -1 && index >= 0)
        {
            StartCoroutine(FadeOut(index));
           
        }
    }

    public void FadeInTrack(int index)
    {
        //if (index < musicTracks.Length - 1 && index >= 0)
        {
            StartCoroutine(FadeIn(index));
        }
    }

    public void PlayTrack(int index)
    {
       // if (index < musicTracks.Length - 1 && index >= 0)
        {
            musicTracks[index].volume = maxVolume[index];
            musicTracks[index].Play();
        }
    }

    IEnumerator FadeOut(int i)
    {
        float timer = 1f;
        musicTracks[i].volume = maxVolume[i];
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            musicTracks[i].volume = maxVolume[i] * (timer / 1f);
            yield return new WaitForSeconds(0.01f);
        }
        musicTracks[i].volume = 0;
        musicTracks[i].Pause();
    }

    IEnumerator FadeIn(int i)
    {
        musicTracks[i].Play();
        musicTracks[i].volume = 0;
        float timer = 2f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            musicTracks[i].volume = maxVolume[i] * ((2f - timer) / 2f);
            yield return new WaitForSeconds(0.01f);
        }
        musicTracks[i].volume = maxVolume[i];
    }
}
