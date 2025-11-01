using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PreGame,
        MainGame
    }

    public static GameManager instance;
    public static GameState CurrentState = GameState.PreGame;




    public List<CharacterController> players;
    public Dictionary<CharacterController, int> playerScores = new Dictionary<CharacterController, int>();

    // Internal dictionary for quick lookup
    private Dictionary<CharacterController, string> playerLookup = new Dictionary<CharacterController, string>();
    


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void AddPlayer(CharacterController newPlayer)
    {
        //if (newPlayer == null) return;
        players.Add(newPlayer);
        int playerCount = players.Count;
        string playerName = "Player" + playerCount;

        playerLookup[newPlayer] = playerName;
        playerScores[newPlayer] = 0;

        //Show playerUI
        UIManager.Instance.UI_UpdatePersonalScoreBoards(players.Count);
        //UIManager.Instance.playerScoreBoards[players.IndexOf(newPlayer)].SetActive(true); 

        Debug.Log($"Added {playerName}");
    }

    public void RemovePlayer(CharacterController player)
    {
        if (playerLookup.ContainsKey(player))
            playerLookup.Remove(player);
        
        //UIManager.Instance.playerScoreBoards[players.IndexOf(player)].SetActive(false);
        players.Remove(player);
        UIManager.Instance.UI_UpdatePersonalScoreBoards(players.Count);
    }

    public string PlayerToName(CharacterController player)
    {
        if (playerLookup.TryGetValue(player, out string name))
        {
            return name;
        }
        else
        {
            return "Unknown";
        }
    }

    //getting top 3 players
    public List<KeyValuePair<CharacterController,int>> GetTop3Players() 
    {
        return playerScores.OrderByDescending(pair => pair.Value).Take(3).ToList();
    }



    public void StartGame()
    {
        //Add ConeSpawer
        // Start Timer
        GameManager.CurrentState = GameState.MainGame;
    }

    public void EndGame()
    {
        // Reload Scene
    }

    internal void addScore(CharacterController owner, int score)
    {
        playerScores[owner] += score;
        Debug.Log("Player: " + PlayerToName(owner) + " Score: " + playerScores[owner]);
    }




}
