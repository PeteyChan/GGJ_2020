using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/SceneLoader")]
public class SceneLoader : ScriptableObject
{
    public SceneField Title;
    public SceneField CharacterSelect;
    public SceneField Game;
    public SceneField End;

    static SceneLoader _instance;
    static SceneLoader Instance
    {
        get
        {
            if (!_instance) _instance = Resources.Load<SceneLoader>("SceneLoader");
            return _instance;
        }
    }

    public static void ToGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.Game);
    }

    public static void ToCharacterSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.CharacterSelect);
    }
}
