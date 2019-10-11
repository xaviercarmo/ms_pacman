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

    public static KeyCode LastMovementKeyPressed { get; private set; }


    private void Start()
    {
        LoadMovementKeys();
        LastMovementKeyPressed = Controls.Right;
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
        //later will load from player prefs and set controls too
        movementKeys[0] = Controls.Up;
        movementKeys[1] = Controls.Left;
        movementKeys[2] = Controls.Down;
        movementKeys[3] = Controls.Right;
    }

    //Gets the key that was last pressed out of the movement keys, if none were pressed then rely on default
    KeyCode GetLastMovementKeyPressed(KeyCode[] keys)
    {
        var result = keys.Where(key => Input.GetKeyDown(key)).LastOrDefault();
        return result == KeyCode.None ? LastMovementKeyPressed : result;
    }

    //erases the last movement key and returns a copy of it
    public static KeyCode ConsumeLastMovementKey()
    {
        var key = LastMovementKeyPressed;
        LastMovementKeyPressed = KeyCode.None;
        return key;
    }
}
