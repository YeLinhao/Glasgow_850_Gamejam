using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public static PlayerInputManager instance;

    private HashSet<Gamepad> joinedGamepads;
    private HashSet<string> joinedKeyboardSchemes;

    // Define keyboard join keys and control schemes
    private (Key key, string scheme)[] keyboardJoinSchemes = new (Key, string)[]
    {
        (Key.C, "WASD"),
        (Key.RightShift, "ArrowKeys"),
        (Key.G, "5RTY"),
        (Key.Period, "IJKL")
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        joinedGamepads = new HashSet<Gamepad>();
        joinedKeyboardSchemes = new HashSet<string>();
    }

    void Update()
    {
        if (GameManager.CurrentState != GameManager.GameState.PreGame)
            return;

        // --- Handle keyboard joins ---
        foreach (var (key, scheme) in keyboardJoinSchemes)
        {
            if (!joinedKeyboardSchemes.Contains(scheme) && Keyboard.current[key].wasPressedThisFrame)
            {
                JoinKeyboardPlayer(scheme);
            }
        }
      

        // --- Handle gamepad joins ---
        foreach (var gamePad in Gamepad.all)
        {
            if (gamePad.buttonSouth.wasPressedThisFrame && !joinedGamepads.Contains(gamePad))
            {
                JoinGamepadPlayer(gamePad);
            }
        }
    }

    private void JoinKeyboardPlayer(string controlScheme)
    {
        Debug.Log(controlScheme);
        var player = PlayerInput.Instantiate(playerPrefab,
            controlScheme: controlScheme,
            pairWithDevice: Keyboard.current);

        AssignSpawnPoint(player, joinedGamepads.Count + joinedKeyboardSchemes.Count);
        joinedKeyboardSchemes.Add(controlScheme);
        Debug.Log(player.currentControlScheme);
    }

    private void JoinGamepadPlayer(Gamepad gamePad)
    {
        var player = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamePad);

        AssignSpawnPoint(player, joinedGamepads.Count + joinedKeyboardSchemes.Count);
        joinedGamepads.Add(gamePad);
    }

    private void AssignSpawnPoint(PlayerInput player, int index)
    {
        if (spawnPoints.Length == 0) return;

        var spawnIndex = Mathf.Clamp(index, 0, spawnPoints.Length - 1);
        var controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            GameManager.instance.AddPlayer(controller);
            controller.enabled = false;
            player.transform.position = spawnPoints[spawnIndex].position;
            controller.enabled = true;
        }
    }

    public void RemovePlayer(PlayerInput player)
    {
        if (player == null) return;

        foreach (var device in player.devices)
        {
            if (device is Gamepad gamepad)
            {
                joinedGamepads.Remove(gamepad);
            }
            else if (device is Keyboard)
            {
                // Find which scheme this player used
                var scheme = player.currentControlScheme;
                joinedKeyboardSchemes.Remove(scheme);
            }
        }

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            GameManager.instance.RemovePlayer(controller);
        }

        Destroy(player.gameObject);
    }
}
