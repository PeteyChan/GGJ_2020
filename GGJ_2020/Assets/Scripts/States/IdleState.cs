using UnityEngine;

public class IdleState : State
{
    private Rigidbody2D _rb;
    private GamePad _pad;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _pad = gameObject.Find<Player>().GamePad;
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (_pad.GetButton(GamePad.Buttons.face_left).wasPressed)
        {
            return GetComponent<PickupState>();
        }

        if (_rb.velocity.magnitude > 0.1f)
        {
            return gameObject.Find<WalkState>();
        }

        return this;
    }
}
