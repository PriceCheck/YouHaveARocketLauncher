using UnityEngine;
using System.Collections;

public class GameplayManager : MonoBehaviour {
    public ScoreUI player1ScoreUI;
    public ScoreUI player2ScoreUI;
    public AmmoUI player1AmmoUI;
    public AmmoUI player2AmmoUI;
    public int MaxScore = 3;
    public GameObject winnerIndicator;
    DontDestroy persistScript;
    int player1Score = 0;
    int player2Score = 0;


	// Use this for initialization
	void Start () {

        if (GameObject.FindGameObjectWithTag("GameplayUI") != null)
        {
            Destroy(gameObject);//So we don't have mulitples
        }
        gameObject.tag = "GameplayUI";
        persistScript = GetComponent<DontDestroy>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            GameObject obj = (GameObject)Instantiate(winnerIndicator);
            obj.GetComponent<WinnerIndcator>().Winner = 1;
            StartCoroutine(Animation());
            GameObject.FindObjectOfType<LevelManager>().LoadLevel(2);//Victory Screen
        }
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            GameObject obj = (GameObject)Instantiate(winnerIndicator);
            obj.GetComponent<WinnerIndcator>().Winner = 2;
            StartCoroutine(Animation());
            GameObject.FindObjectOfType<LevelManager>().LoadLevel(2);//Victory Screen
        }
	}

    public AmmoUI GetAmmoUI(int player)
    {
        if(player == 1)
        {
            return player1AmmoUI;
        }
        else
        {
            return player2AmmoUI;
        }
    }

    public void HasScored(int player)
    {
        if(player == 1)
        {
            ++player1Score;
            player1ScoreUI.HasScored();
        }
        else if(player == 2)
        {
            ++player2Score;
            player2ScoreUI.HasScored();
        }
        if(CheckWinCondition())
        {
            
            StartCoroutine(Animation());
            GameObject.FindObjectOfType<LevelManager>().LoadLevel(2);//Victory Screen
            
        }
        else
        {
            GameObject.FindObjectOfType<LevelManager>().RestartLevel();//Victory Screen
        }
    }

    bool CheckWinCondition()
    {

        if(player1Score >= MaxScore)
        {
            GameObject obj = (GameObject)Instantiate(winnerIndicator);
            obj.GetComponent<WinnerIndcator>().Winner = 1;
            return true;
        }
        else if(player2Score >= MaxScore)
        {
            GameObject obj = (GameObject)Instantiate(winnerIndicator);
            obj.GetComponent<WinnerIndcator>().Winner = 2;
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        
    }

    IEnumerator Animation()
    {
        float timer = 0.75f;
        while (timer > 0)
        {
            GetComponent<CanvasGroup>().alpha = timer / 0.75f;
            timer -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<CanvasGroup>().alpha = 0;
        Destroy(gameObject);
    }
}
