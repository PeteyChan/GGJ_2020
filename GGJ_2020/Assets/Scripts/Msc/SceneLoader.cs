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
        foreach (var item in ShipSpawner.spawners)
        {
            Destroy(item);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.Game);
        var spawner = new GameObject().AddComponent<ShipSpawner>();
        spawner.Team = GameSettings.Team.Blue;
        DontDestroyOnLoad(spawner);

        spawner = Instantiate(spawner);
        spawner.Team = GameSettings.Team.Red;
        DontDestroyOnLoad(spawner);
    }

    public static void ToEnd()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.End);
    }

    public static void ToCharacterSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.CharacterSelect);
    }

    public static void ToTitleScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Instance.Title);
    }
}
