using UnityEngine;
using System.Collections;
using UnityEngine.Audio;


public class LevelSelectButton3 : MonoBehaviour
{

    LevelManager m_LevelManager;

    public int sceneToStart = 1;                                        //Index number in build settings of scene to load if changeScenes is true

    void Start()
    {
        m_LevelManager = FindObjectOfType(typeof(LevelManager)) as LevelManager;
    }


    public void StartButtonClicked()
    {

        //Call the StartGameInScene function to start game without loading a new scene.
        StartGameInScene();
    }


    public void LoadDelayed()
    {
        //Load the selected scene, by scene index number in build settings
        Application.LoadLevel(sceneToStart);
    }


    public void StartGameInScene()
    {


        m_LevelManager.LoadLevel(sceneToStart);
    }


    public void PlayNewMusic()
    {

    }

    public void HideDelayed()
    {

    }
}
