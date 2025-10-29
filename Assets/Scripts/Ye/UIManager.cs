using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

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


    public void UI_EnableLeaderboard()
    {
        LeaderboardPanel.SetActive(true);
        FirstPlace_Score.text = GameManager.instance.GetTop3Players()[0].Value.ToString();
        SecondPlace_Score.text = GameManager.instance.GetTop3Players()[1].Value.ToString();
        ThirdPlace_Score.text = GameManager.instance.GetTop3Players()[2].Value.ToString();
    }


    private void Update()
    {
        //foreach (var item in GameManager.instance.players)
        //{
        //    item.
        //    playersScore[]
        //}


        if (GameManager.instance.players.Count != 0) 
        {
            int index = 0;//index of player in loop
            foreach (var item in GameManager.instance.players)
            {
                playersScore[index].text = "Score" + GameManager.instance.playerScores[item].ToString();
                index++;
            }
            index = 0;


        }
    }


}
