using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchScene : MonoBehaviour
{
    public SceneField Scene;

    public void Launch()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scene);
    }
}
