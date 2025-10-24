using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;



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

        Debug.Log($"Added {playerName}");
    }

    public void RemovePlayer(CharacterController player)
    {
        if (playerLookup.ContainsKey(player))
            playerLookup.Remove(player);
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

    internal void addScore(CharacterController owner, int score)
    {
        playerScores[owner] += score;
        Debug.Log("Player: " + PlayerToName(owner) + " Score: " + playerScores[owner]);
    }
}
