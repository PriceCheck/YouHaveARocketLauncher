using UnityEngine;
using System.Collections;

public class CreditsExampleScript : MonoBehaviour
{
    LevelManager m_LevelManager;
    void Start()
	{
        // Starting manually the credit roll if "Play On Awake" is false:
        Credits.getInstance().beginCredits();

        // You can also do localization with PlayOnAwake false and setting the file before starting:
        //Credits c = Credits.getInstance();
        //c.creditsFile = Resources.Load<TextAsset>("credits/enEN");
        //c.beginCredits();
        m_LevelManager = FindObjectOfType(typeof(LevelManager)) as LevelManager;
        // Callback
        Credits.getInstance().endListeners += new Credits.CreditsEndListener(creditsEnded); // creditsEnded is the name of the function
	}
	
	void creditsEnded(Credits c)
	{
        m_LevelManager.LoadLevel(0);
    }
}
