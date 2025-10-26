using UnityEngine;
using UnityEngine.UI;
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

    public TMP_Text Player1_Score;


    public void UI_EnableLeaderboard()
    {
        LeaderboardPanel.SetActive(true);
    }


    private void Update()
    {
        Player1_Score.text = "Score" + GameManager.instance.playerScores[GameManager.instance.players[0]].ToString(); 
    }


}
