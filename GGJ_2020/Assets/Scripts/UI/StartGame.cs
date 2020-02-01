using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    UnityEngine.UI.Text Text;

    public SceneField GameScene;

    private void Start()
    {
        Text = GetComponent<UnityEngine.UI.Text>();
        Text.text = "";
    }

    private void Update()
    {
        if (GameSettings.CanStart())
        {
            Text.text = "Start";
        }
        else Text.text = "";
    }
}
