using UnityEngine;
using System.Collections;

public class VictoryLevel : MonoBehaviour {
    Vector3 WinnerPos;

    public GameObject destructablePrefab;
    public Material Bottom;
    public Material Tier1;
    public Material Tier2;
    public Material TierFinal;
    public GameObject player1;
    public GameObject player2;
    private bool isInit = false;
    public int level = 0;
    // Use this for initialization
    void Start () {
        WinnerPos = new Vector3(37f, 12.7f, 8);    
	}
	
	// Update is called once per frame
	void Update () {
        if (isInit)
        {
            return;
        }
        CreateWinScreen();
        isInit = true;
    }
    void VictoryLevelCode()
    {

    }

    void CreateWinScreen()
    {
        VictoryLevelCode();
        WinnerIndcator win = GameObject.FindObjectOfType<WinnerIndcator>();
        Vector3 player1Pos = new Vector3(0,-1000,0);
        Vector3 player2Pos = new Vector3(0, -1000, 0);
        if (win != null && win.Winner == 1)
        {
            //player 1 wins
            player1Pos = WinnerPos;
        }
        else
        {
            //player 2 wins
            player2Pos = WinnerPos;
        }

        int[][] Level4;
        Level4 = new int[10][];
        Level4[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Level4[1] = new int[] { 0, 0, 4, 0, 0, 0, 0, 0, 2, 0 };
        Level4[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Level4[3] = new int[] { 0, 0, 0, 0, 2, 2, 2, 0, 0, 0 };
        Level4[4] = new int[] { 0, 0, 2, 1, 3, 5, 2, 0, 0, 0 };
        Level4[5] = new int[] { 0, 0, 0, 1, 3, 3, 2, 2, 0, 0 };
        Level4[6] = new int[] { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 };
        Level4[7] = new int[] { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 };
        Level4[8] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Level4[9] = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };

        CreateLevelFunc(Level4, 1.75f, new Vector3(30, 4, 0), player1Pos, player2Pos, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

    }

    void CreateLevelFunc(int[][] data, float scale, Vector3 startingPos, Vector3 play1pos, Vector3 play2pos, Vector3 rotation1, Vector3 rotation2)
    {
        Vector3 newScale = new Vector3(scale, scale, scale);
        for (int i = 0; i < data.Length; ++i)
        {
            for (int j = 0; data[i] != null && j < data[i].Length; ++j)
            {
                for (int k = 0; k < data[i][j]; ++k)
                {
                    Vector3 newPos = new Vector3(startingPos.x + (i * scale), startingPos.y + (k * scale), startingPos.z + (j * scale));
                    GameObject obj = (GameObject)Instantiate(destructablePrefab, newPos, Quaternion.identity);
                    obj.transform.localScale = newScale;
                    if (k == 0)
                    {
                        obj.GetComponent<MeshRenderer>().material = Bottom;
                    }
                    else if (k == 1)
                    {
                        obj.GetComponent<MeshRenderer>().material = Tier1;
                    }
                    else if (k == 2)
                    {
                        obj.GetComponent<MeshRenderer>().material = Tier2;
                    }
                    else
                    {
                        obj.GetComponent<MeshRenderer>().material = TierFinal;
                    }
                }
            }

        }
        GameObject obj1 = (GameObject) GameObject.Instantiate(player1);
        obj1.transform.position = play1pos;
        obj1.transform.rotation = Quaternion.Euler(rotation1.x, rotation1.y, rotation1.z);

        GameObject obj2 = (GameObject)GameObject.Instantiate(player2);
        obj2.transform.position = play2pos;
        obj2.transform.rotation = Quaternion.Euler(rotation2.x, rotation2.y, rotation2.z);

    }
}
