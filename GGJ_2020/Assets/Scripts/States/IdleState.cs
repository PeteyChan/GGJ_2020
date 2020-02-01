using UnityEngine;

[RequireComponent(typeof(WalkState))]
[RequireComponent(typeof(DashState))]
public class IdleState : State
{
    private Player _player;

    private void Awake()
    {
        _player = gameObject.Find<Player>();
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (_player.GamePad.GetButton(GamePad.Buttons.face_left).wasPressed)
        {
            return gameObject.Find<PickupState>();
        }

        if (_player.Rigidbody.velocity.magnitude > 0.1f)
        {
            return gameObject.Find<WalkState>();
        }

        return this;
    }
}
