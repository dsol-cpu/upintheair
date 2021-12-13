using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return; // No gamepad connected.

        if (keyboard.sKey.wasPressedThisFrame)
        {
            // 'Use' code here
        }

        // 'Move' code here
    }
}