using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Set as Singleton
    public static UIManager Instance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject LeaderboardPanel;
    public TMP_Text FirstPlace_Score;
    public TMP_Text SecondPlace_Score;
    public TMP_Text ThirdPlace_Score;

    public List<GameObject> playerScoreBoards;
    public List<TMP_Text> playersScore;
    public Slider GameStartProgress360;

    public List<Material> PlayerOutlines;


    //When game ends
    public void UI_EnableLeaderboard()
    {
        LeaderboardPanel.SetActive(true);

        Debug.Log("PLAYER NUM: " + GameManager.instance.GetTop3Players().Count);

        FirstPlace_Score.text = GameManager.instance.GetTop3Players()[0].Value.ToString();

        if (GameManager.instance.GetTop3Players().Count >= 2)
        {
            SecondPlace_Score.text = GameManager.instance.GetTop3Players()[1].Value.ToString();
        }

        if (GameManager.instance.GetTop3Players().Count >= 3)
        {
            ThirdPlace_Score.text = GameManager.instance.GetTop3Players()[2].Value.ToString();
        }
   
    }

    //When prepare stage, player join/quit
    public void UI_UpdatePersonalScoreBoards(int playerNumbers)
    {
        foreach (var item in playerScoreBoards)
        {
            item.SetActive(false);
        }

        for (int i = 0; i < playerNumbers; i++)
        {
            playerScoreBoards[i].SetActive(true);
        }

    }

    public void UI_UpdatePlayerColor(int playerNumbers)
    {
        for (int i = 0; i < playerNumbers; i++)
        {
            //var rend = GameManager.instance.players[i].transform.GetChild(0).GetComponent<MeshRenderer>();
            //var mats = rend.materials;   
            //mats[1] = PlayerOutlines[i]; 
            //rend.materials = mats;

            //GameManager.instance.players[i].transform.GetChild(0).GetComponent<MeshRenderer>().materials[1] = PlayerOutlines[i];

            Debug.Log("Player" + (i + 1).ToString() + "has changed color!");


        }

    }


    private void Update()
    {

        //Update player regular score board
        if (GameManager.instance.players.Count != 0) 
        {
            int index = 0;//index of player in loop
            foreach (var item in GameManager.instance.players)
            {
                playersScore[index].text = GameManager.instance.playerScores[item].ToString();
                index++;
            }
            index = 0;


        }
    }


    public void Scene_Restart()
    {
        SceneManager.LoadScene(1);
        GameManager.CurrentState = GameManager.GameState.PreGame;
    }

    public void Scene_BackMenu()
    {
        SceneManager.LoadScene(0);
        GameManager.CurrentState = GameManager.GameState.PreGame;
    }

}
