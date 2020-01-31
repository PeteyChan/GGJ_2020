using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    GamePad pad = new GamePad(1);
    GamePad pad2 = new GamePad(5);

    // Update is called once per frame
    void Update()
    {
        if (pad.GetButton(GamePad.Buttons.face_up).isPressed)
            Debug.Log("Foo");
        if (pad2.GetButton(GamePad.Buttons.lHat).wasPressed)
            Debug.Log("Wee");
    }
}
