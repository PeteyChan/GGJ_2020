using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(PickupState))]
[RequireComponent(typeof(DropState))]
public class CarryWalkState : State
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
            return GetComponent<DropState>();
        }

        if (_player.Rigidbody.velocity.magnitude < 0.1f)
        {
            return GetComponent<CarryIdleState>();
        }

        return this;
    }
}
