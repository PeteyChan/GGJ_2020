using UnityEngine;

[CreateAssetMenu(menuName = "Settings/GameSounds")]
public class GameSounds : ScriptableObject
{
    private static GameSounds _instance;
    public static GameSounds Instance => _instance != null ? _instance : (_instance = Resources.Load<GameSounds>("GameSounds"));

    public AudioClip Dash;
    public AudioClip KnockDown;
    public AudioClip PartPlacement;
    public AudioClip Lifting;
    public AudioClip GameStart;
    public AudioClip GameOver;

    public AudioClip BackgroundMusic;
}
