using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private HashSet<Gamepad> joinedGamepads = new HashSet<Gamepad>();

    private bool wasdJoined = false;


    // Update is called once per frame
    void Update()
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
