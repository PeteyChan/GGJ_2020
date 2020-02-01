using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMoveDirection : MonoBehaviour
{
    Rigidbody Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.Find<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = Rigidbody.velocity;
        dir.y = 0;
        if (dir.magnitude < .1f)
            return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime * 5f);
    }
}
