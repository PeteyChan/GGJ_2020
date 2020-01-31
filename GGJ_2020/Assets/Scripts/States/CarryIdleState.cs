using UnityEngine;

public class CarryIdleState : State
{
    private Rigidbody2D _rb;
    private GamePad _pad;

    private void Awake()
    {
        _rb = gameObject.Find<Rigidbody2D>();
        _pad = gameObject.Find<Player>().GamePad;
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (_pad.GetButton(GamePad.Buttons.face_left).wasPressed)
        {
            return GetComponent<DropState>();
        }
        
        if (_rb.velocity.magnitude > 0.1f)
        {
            return GetComponent<CarryMoveState>();
        }

        return this;
    }
}
