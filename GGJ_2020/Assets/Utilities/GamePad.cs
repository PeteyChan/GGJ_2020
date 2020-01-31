using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class GamePad
{
    public GamePad(int player)
    {
        this.player = Mathf.Max(1, player);
    }

    public int player
    { get; private set; }

    static Gamepad NullGamepad = new Gamepad();
    Gamepad gamepad
    {
        get
        {
            var all = Gamepad.all;
            if (all.Count < player || player < 1)
                return NullGamepad;
            return all[player - 1];
        }
    }

    public State GetButton(Buttons button)
    {
        switch (button)
        {
            case Buttons.dpad_up:
                return ToState(gamepad.dpad.up);
            case Buttons.dpad_down:
                return ToState(gamepad.dpad.down);
            case Buttons.dpad_left:
                return ToState(gamepad.dpad.left);
            case Buttons.dpad_right:
                return ToState(gamepad.dpad.right);

            case Buttons.face_up:
                return ToState(gamepad.buttonNorth);
            case Buttons.face_down:
                return ToState(gamepad.buttonSouth);
            case Buttons.face_left:
                return ToState(gamepad.buttonWest);
            case Buttons.face_right:
                return ToState(gamepad.buttonEast);

            case Buttons.lShoulder:
                return ToState(gamepad.leftShoulder);
            case Buttons.rShoulder:
                return ToState(gamepad.rightShoulder);

            case Buttons.lTrigger:
                return ToState(gamepad.leftTrigger);
            case Buttons.rTrigger:
                return ToState(gamepad.rightTrigger);

            case Buttons.lHat:
                return ToState(gamepad.leftStickButton);
            case Buttons.rHat:
                return ToState(gamepad.rightStickButton);

            case Buttons.start:
                return ToState(gamepad.startButton);
            case Buttons.select:
                return ToState(gamepad.selectButton);

            case Buttons.lJoyStick_up:
                return ToState(gamepad.leftStick.up);
            case Buttons.lJoyStick_down:
                return ToState(gamepad.leftStick.down);
            case Buttons.lJoyStick_left:
                return ToState(gamepad.leftStick.left);
            case Buttons.lJoyStick_right:
                return ToState(gamepad.leftStick.right);

            case Buttons.rJoyStick_up:
                return ToState(gamepad.rightStick.up);
            case Buttons.rJoyStick_down:
                return ToState(gamepad.rightStick.down);
            case Buttons.rJoyStick_left:
                return ToState(gamepad.rightStick.left);
            case Buttons.rJoyStick_right:
                return ToState(gamepad.rightStick.right);

            default:
                return new State();
        }
    }

    public Vector2 LeftStick
    {
        get
        {
            return gamepad.leftStick.ReadValue();
        }
    }

    public Vector2 RightStick
    {
        get
        {
            return gamepad.rightStick.ReadValue();
        }
    }

    public void SetVibration(float left, float right)
    {
        gamepad.SetMotorSpeeds(left, right);
    }

    State ToState(UnityEngine.InputSystem.Controls.ButtonControl control)
    {
        var state = new State();
        state.value = control.ReadValue();
        state.wasPressed = control.wasPressedThisFrame;
        state.wasReleased = control.wasReleasedThisFrame;
        return state;
    }

    public struct State
    {
        /// <summary>
        /// current Value of button State
        /// </summary>
        public float value;
        /// <summary>
        /// returns true if pressed this frame
        /// </summary>
        public bool wasPressed;
        /// <summary>
        /// returns true if currently pressed
        /// </summary>
        public bool isPressed => value > 0.1f;
        /// <summary>
        /// returns true if released this frame
        /// </summary>
        public bool wasReleased;
        /// <summary>
        /// returns true if currently released
        /// </summary>
        public bool isReleased => value < 0.1f;
    }

    public enum Buttons
    {
        dpad_up,
        dpad_down,
        dpad_left,
        dpad_right,

        face_up,
        face_down,
        face_left,
        face_right,

        lShoulder,
        rShoulder,

        lTrigger,
        rTrigger,

        lHat,
        rHat,

        lJoyStick_up,
        lJoyStick_down,
        lJoyStick_left,
        lJoyStick_right,

        rJoyStick_up,
        rJoyStick_down,
        rJoyStick_left,
        rJoyStick_right,

        start,
        select,
    }
}