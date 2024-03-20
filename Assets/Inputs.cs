using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public int gamepadIndex = -1;
    public int InvalidGamepadIndex = -1;

    Vector2 inputDirection = Vector2.zero;
    float inputMagnitude = 0.0f;

    public void SetGamepadIndex(int gamepadIndex)
    {
        this.gamepadIndex = gamepadIndex;
    }
    public bool HasKeyboard()
    {
        Debug.Assert(gamepadIndex != InvalidGamepadIndex);
        return gamepadIndex == 0;
    }
    public int GamepadIndex()
    {
        Debug.Assert(gamepadIndex != InvalidGamepadIndex);
        return gamepadIndex;
    }
    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }
    public float GetInputMagnitude()
    {
        return inputMagnitude;
    }
    public void LateUpdate()
    {
        if (gamepadIndex == InvalidGamepadIndex) return;
        

        float hz = Input.GetAxis($"Horizontal-GP-{gamepadIndex}") + Input.GetAxis($"Horizontal");
        float vt = Input.GetAxis($"Vertical-GP-{gamepadIndex}") + Input.GetAxis($"Vertical");

        inputDirection = new Vector2(hz, vt);
        inputMagnitude = inputDirection.magnitude;
        inputDirection.Normalize();
    }
}
