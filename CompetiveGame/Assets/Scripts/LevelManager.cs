using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
    private MusicManager m_MusicManager;
    public bool playMusicOnStart = false;
    public int LevelMusicIndex = 0;
    public int LevelNumber = 0;
    public float FadeTime = 2f;

	// Use this for initialization
	void Start () {
        if (playMusicOnStart)
        {
            //print("You a playa");
            m_MusicManager = FindObjectOfType(typeof(MusicManager)) as MusicManager;
            m_MusicManager.FadeInTrack(LevelMusicIndex);
        }
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 0.1f, () => {  });
    }
	
    public void LoadLevel(int index)
    {
        CameraFade.StartAlphaFade(Color.black, false, FadeTime, 0.2f, () => { Application.LoadLevel(index); });
        if (playMusicOnStart)
        {
            m_MusicManager.FadeOutTrack(LevelMusicIndex);
        }
    }

    public void RestartLevel()
    {
        CameraFade.StartAlphaFade(Color.black, false, FadeTime, 0.2f, () => { Application.LoadLevel(Application.loadedLevel); });
    }
    public void EnterNewlevel(int index)
    {
   
    //CameraFade.StartAlphaFade(Color.black, true, 1f, 0.0f, () => { });
    }
	// Update is called once per frame
	void Update () {
	
	}

    

}
