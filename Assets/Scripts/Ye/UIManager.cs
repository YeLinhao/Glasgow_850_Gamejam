using UnityEngine;

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

    public void UI_EnableLeaderboard()
    {
        LeaderboardPanel.SetActive(true);
    }



}
