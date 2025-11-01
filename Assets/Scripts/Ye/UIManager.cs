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
    }

    public void Scene_BackMenu()
    {
        SceneManager.LoadScene(0);
    }

}
