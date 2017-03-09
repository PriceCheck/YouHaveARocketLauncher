using UnityEngine;
using System.Collections;
using Text = UnityEngine.UI.Text;

public class ScoreUI : MonoBehaviour {
    private Text UI;
    public int CurrentScore = 0;
    // Use this for initialization
	void Start () {
        UI = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int HasScored()
    {
        ++CurrentScore;
        UI.text = "" + CurrentScore;
        return CurrentScore;
    }

    

}
