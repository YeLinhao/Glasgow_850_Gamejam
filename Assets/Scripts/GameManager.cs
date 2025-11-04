using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PreGame,
        MainGame
    }

    public enum WeatherType
    {
        Clear,
        Rain,
        Wind
    }

    public static GameManager instance;
    public static GameState CurrentState = GameState.PreGame;
    public WeatherType currentWeather = WeatherType.Rain;
    private ParticleSystem rainParticles;
    public ParticleSystem rainParticlesPrefab;

    [Header("Weather Forces")]
    public Vector3 windDirection = new Vector3(1f, 0f, 0f);
    public float windForce = 0.5f;
    public float rainForce = 1.0f;
    public GameObject windTrailPrefab;
    public float windTrailSpawnRate = 0.5f; // seconds between trails
    public Vector3 windAreaSize = new Vector3(30f, 10f, 30f); // spawn area
    public float windDuration;
    private Coroutine windCoroutine;
    private Coroutine weatherChangeRoutine;

    private bool hasWeatherStarted = false;

    public GameObject timer;


    public List<CharacterController> players;
    public Dictionary<CharacterController, int> playerScores = new Dictionary<CharacterController, int>();

    // Internal dictionary for quick lookup
    private Dictionary<CharacterController, string> playerLookup = new Dictionary<CharacterController, string>();
    


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        GameManager.CurrentState = GameManager.GameState.PreGame;
    }
    public void AddPlayer(CharacterController newPlayer)
    {
        //if (newPlayer == null) return;
        players.Add(newPlayer);
        int playerCount = players.Count;
        string playerName = "Player" + playerCount;

        playerLookup[newPlayer] = playerName;
        playerScores[newPlayer] = 0;


        //Spawn Sfx
        newPlayer.GetComponent<PlayerAudioControl>().spawnSounds.Play();

        //Show playerUI
        UIManager.Instance.UI_UpdatePersonalScoreBoards(players.Count);
        UIManager.Instance.UI_UpdatePlayerColor(players.Count);


        Debug.Log($"Added {playerName}");
    }

    public void RemovePlayer(CharacterController player)
    {
        if (playerLookup.ContainsKey(player))
            playerLookup.Remove(player);

        players.Remove(player);
        UIManager.Instance.UI_UpdatePersonalScoreBoards(players.Count);
        UIManager.Instance.UI_UpdatePlayerColor(players.Count);
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

    private void Update()
    {
        HandleRainState();
        HandleWindState();

        // --- Start weather randomization when 60 seconds remain ---
        if (!hasWeatherStarted && TimerWithTMPro.currentTime <= 60f)
        {
            hasWeatherStarted = true;
            StartWeatherSystem();
        }
    }
    private void HandleRainState()
    {
        if (currentWeather == WeatherType.Rain)
        {
            // Instantiate rain if it doesn't exist yet
            if (rainParticles == null && rainParticlesPrefab != null)
            {
                rainParticles = Instantiate(rainParticlesPrefab, transform);
            }

            if (rainParticles != null && !rainParticles.isPlaying)
            {
                rainParticles.Play();
            }
        }
        else
        {
            if (rainParticles != null && rainParticles.isPlaying)
            {
                rainParticles.Stop();
            }
        }
    }

    private void HandleWindState()
    {
        if (currentWeather == WeatherType.Wind && windCoroutine == null)
        {
            windCoroutine = StartCoroutine(WindTrailRoutine());
        }
        else if (currentWeather != WeatherType.Wind && windCoroutine != null)
        {
            StopCoroutine(windCoroutine);
            windCoroutine = null;
        }
    }

    private void StartWeatherSystem()
    {
        // Step 1: choose Rain or Wind randomly
        currentWeather = (UnityEngine.Random.value < 0.5f) ? WeatherType.Rain : WeatherType.Wind;
        ApplyWeather();

        // Step 2: start periodic randomizer
        weatherChangeRoutine = StartCoroutine(WeatherRandomizerRoutine());
    }

    private IEnumerator WeatherRandomizerRoutine()
    {
        while (TimerWithTMPro.currentTime > 0f)
        {
            yield return new WaitForSeconds(15f);
            currentWeather = (WeatherType)UnityEngine.Random.Range(0, 3);
            ApplyWeather();
        }
    }

    private void ApplyWeather()
    {
        HandleRainState();
        HandleWindState();
        Debug.Log($"Weather changed to: {currentWeather}");
    }

    IEnumerator WindTrailRoutine()
    {
        while (true)
        {
            if (currentWeather == WeatherType.Wind && windTrailPrefab != null)
            {
                SpawnWindTrail();
            }

            yield return new WaitForSeconds(windTrailSpawnRate);
        }
    }

    void SpawnWindTrail()
    {
        Vector3 spawnPos = new Vector3(
            UnityEngine.Random.Range((-windAreaSize.x / 2) - 0, (windAreaSize.x / 2)-0),
            UnityEngine.Random.Range(3, windAreaSize.y),
            UnityEngine.Random.Range(-windAreaSize.z / 2, windAreaSize.z / 2)
        );

        GameObject trail = Instantiate(windTrailPrefab, spawnPos, Quaternion.identity);
        trail.transform.forward = windDirection.normalized;
        Destroy(trail, windDuration); // cleanup after a few seconds

        Rigidbody rb = trail.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = trail.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
        }

        rb.linearVelocity = windDirection.normalized * (windForce * 10f);
    }




}
