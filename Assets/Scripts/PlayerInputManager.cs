using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private HashSet<Gamepad> joinedGamepads = new HashSet<Gamepad>();



    // Update is called once per frame
    void Update()
    {
        foreach (var gamePad in Gamepad.all)
        {
            if (gamePad.buttonSouth.wasPressedThisFrame && !joinedGamepads.Contains(gamePad))
            {
                var player = PlayerInput.Instantiate(playerPrefab,
                    controlScheme: "Gamepad",
                    pairWithDevice: gamePad);


                if (spawnPoints.Length > 0)
                {
                    player.transform.position = spawnPoints[joinedGamepads.Count].position;
                }
                joinedGamepads.Add(gamePad);
            }
        }
    }
}
