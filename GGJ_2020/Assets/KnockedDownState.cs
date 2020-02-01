using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockedDownState : State
{
    public AnimationClip clip;

    Animate animate;
    Rigidbody Rigidbody;
    Vector3 enterVel;
    Player Player;

    public Vector3 knockedDownForce;

    private void Awake()
    {
        gameObject.TryFind(out Player);
        gameObject.TryFind(out animate);
        gameObject.TryFind(out Rigidbody);
    }
    
    protected override void OnEnter()
    {
        enterVel = Rigidbody.velocity;

        droppedPart = false;
        animate.Play(clip, .1f);
        animate.Speed = 2f;

        if (Player.HeldPart)
        {
            Player.HeldPart.Holder = null;
            Player.HeldPart = null;
            droppedPart = true;
        }
    }

    bool droppedPart;
    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (droppedPart)
        {
            knockedDownForce.y = 0;
            animate.transform.forward = -knockedDownForce;
            Rigidbody.velocity = Vector3.Lerp(-knockedDownForce/4f, Vector3.zero, stateTime);
            if (stateTime > knockedDownForce.magnitude/2f)
                return null;
        }
        else
        {
            enterVel.y = 0;
            Rigidbody.velocity = Vector3.Lerp(-animate.transform.forward*enterVel.magnitude/4f, Vector3.zero, stateTime);
            if (stateTime > enterVel.magnitude/4f)
                return null;
        }
        return this;
    }

}
