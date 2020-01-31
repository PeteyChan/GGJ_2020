using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class GameConsole : GameSystem, Events.IOnGUI, Events.IOnUpdate, Events.IOnConsoleInput
{
    /// <summary>
    /// Logs message to game console
    /// </summary>
    /// <param name="message"></param>
    public static void Log(string message, UnityEngine.Object target = null)
    {
        Debug.Log(message, target);
        log.Add(message);
    }

    /// <summary>
    /// Clears the message log
    /// </summary>
    public static void ClearLog()
    {
        log.Clear();
    }

    string consoleInput = "";
    bool showConsole;
    static List<string> log = new List<string>();

    float backspaceTime = 0;

    public void OnUpdate(float deltaTime)
    {
        if (showFps)
            fps = 1/Time.unscaledDeltaTime;
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            showConsole = !showConsole;
            return;
        }
        if (!showConsole) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(consoleInput))
            {
                string[] input = consoleInput.Split(' ');
                foreach (var system in SystemsWith<Events.IOnConsoleInput>())
                    system.OnConsoleInput(input);
                consoleInput = "";
                return;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                consoleInput = "";
                return;
            }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            backspaceTime = 0;
            if (consoleInput.Length > 0)
                consoleInput = consoleInput.Substring(0, consoleInput.Length - 1);
            return;
        }
        if (Input.GetKey(KeyCode.Backspace))
        {
            backspaceTime += deltaTime;
            if (backspaceTime > .5f && consoleInput.Length > 0)
                consoleInput = consoleInput.Substring(0, consoleInput.Length - 1);
            return;
        }
        consoleInput += Input.inputString;
    }

    bool showFps;
    float fps;
    GUIStyle skin;

    void IOnGUI.OnGUI()
    {
        if (showFps)
        {
            if (skin == null)
            {
                skin = new GUIStyle(GUI.skin.label);
                skin.fontSize = 18;
                skin.alignment = TextAnchor.MiddleLeft;
                skin.fontStyle = FontStyle.Bold;
            }

            if (fps > 60)
                skin.normal.textColor = Color.green;
            else if (fps > 30)
                skin.normal.textColor = Color.yellow;
            else skin.normal.textColor = Color.red;
            GUI.Label(new Rect(Screen.width - 128, Screen.height-32, 128, 32), $"FPS: {fps : 0.0}", skin);
        }

        if (!showConsole) return;

        using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height)))
        {
            var maxLines = Screen.height / GUI.skin.label.lineHeight;

            consoleInput = GUILayout.TextArea(consoleInput, GUILayout.Width(256));
            for (int i = log.Count - 1; i >= 0; --i)
            {
                GUILayout.Label(log[i]);
            }
        }
    }

    public void OnConsoleInput(string[] consoleInput)
    {
        var Event = consoleInput[0];
        switch (consoleInput[0])
        {
            case "fps":
                showFps = !showFps;
                break;
            case "timescale":
                if (float.TryParse(consoleInput[1], out var value))
                {
                    Time.timeScale = Mathf.Max(0, value);
                }
                break;
            case "log":
                if (consoleInput.Length > 1)
                {
                    string val = "";
                    for (int i = 1; i < consoleInput.Length; ++i)
                        val += consoleInput[i] + " ";
                    GameConsole.Log(val);
                }
                break;
            case "clear":
            case "clr":
                GameConsole.ClearLog();
                break;
            case "close":
            case "quit":
#if UNITY_EDITOR
                if (Application.isEditor)
                    UnityEditor.EditorApplication.isPlaying = false;
                else
#endif
                    Application.Quit();
                break;
        }
    }
}

namespace Events
{
    public interface IOnConsoleInput
    {
        void OnConsoleInput(string[] consoleInput);
    }
}

