using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MovementControls
{
    public KeyCode Up;
    public KeyCode Left;
    public KeyCode Down;
    public KeyCode Right;
    public KeyCode Pause;
}

public class PlayerInputManager : MonoBehaviour
{
    public static MovementControls Controls = new MovementControls()
    {
        Up = KeyCode.W,
        Left = KeyCode.A,
        Down = KeyCode.S,
        Right = KeyCode.D,
        Pause = KeyCode.Escape
    };

    KeyCode[] movementKeys = new KeyCode[4];

    public static KeyCode LastMovementKeyPressed = KeyCode.D;


    private void Start()
    {
        LoadMovementKeys();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            LastMovementKeyPressed = GetLastMovementKeyPressed(movementKeys);
        }
    }

    //Called when player preferences are changed to re-initialise the movement key array
    public void LoadMovementKeys()
    {
        //load from player prefs
        movementKeys[0] = Controls.Up;
        movementKeys[1] = Controls.Left;
        movementKeys[2] = Controls.Down;
        movementKeys[3] = Controls.Right;
    }

    //Gets the key that was last pressed out of the movement keys
    KeyCode GetLastMovementKeyPressed(KeyCode[] keys)
        => keys.Where(key => Input.GetKeyDown(key)).LastOrDefault();

    //erases the last movement key and returns a copy of it
    public static KeyCode ConsumeLastMovementKey()
    {
        var key = LastMovementKeyPressed;
        LastMovementKeyPressed = KeyCode.None;
        return key;
    }
}
