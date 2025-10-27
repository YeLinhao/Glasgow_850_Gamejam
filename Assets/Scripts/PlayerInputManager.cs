using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public static PlayerInputManager instance;


    private HashSet<Gamepad> joinedGamepads = new HashSet<Gamepad>();

    private bool wasdJoined = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.CurrentState == GameManager.GameState.PreGame)
        {
            if (Keyboard.current == null) return;
            if (!wasdJoined && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                var player = PlayerInput.Instantiate(playerPrefab,
                        controlScheme: "WASD",
                        pairWithDevice: Keyboard.current);

                if (spawnPoints.Length > 0)
                {
                    // Get the CharacterController component
                    CharacterController controller = player.GetComponent<CharacterController>();
                    GameManager.instance.AddPlayer(controller);
                    // Place the player at the spawn point
                    controller.enabled = false; // temporarily disable to safely set position
                    player.transform.position = spawnPoints[joinedGamepads.Count].position;
                    controller.enabled = true; // re-enable before using Move
                }
                wasdJoined = true;
            }
            foreach (var gamePad in Gamepad.all)
            {
                if (gamePad.buttonSouth.wasPressedThisFrame && !joinedGamepads.Contains(gamePad))
                {
                    var player = PlayerInput.Instantiate(playerPrefab,
                        controlScheme: "Gamepad",
                        pairWithDevice: gamePad);


                    if (spawnPoints.Length > 0)
                    {
                        // Get the CharacterController component
                        CharacterController controller = player.GetComponent<CharacterController>();
                        GameManager.instance.AddPlayer(controller);

                        // Place the player at the spawn point
                        controller.enabled = false; // temporarily disable to safely set position
                        player.transform.position = spawnPoints[joinedGamepads.Count].position;
                        controller.enabled = true; // re-enable before using Move
                    }
                    joinedGamepads.Add(gamePad);
                }
            }
        }
    }

    public void RemovePlayer(PlayerInput player)
    {
        if (player == null) return;

        // --- 1. Unpair the device(s) ---
        var devices = player.devices;
        foreach (var device in devices)
        {
            if (device is Gamepad gamepad)
            {
                // Remove the gamepad from joined set
                joinedGamepads.Remove(gamepad);
            }
            else if (device is Keyboard)
            {
                wasdJoined = false;
            }
        }

        // --- 2. Remove the player from the GameManager list ---
        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            GameManager.instance.RemovePlayer(controller);
        }

        // --- 3. Destroy the player object ---
        Destroy(player.gameObject);
    }
}
