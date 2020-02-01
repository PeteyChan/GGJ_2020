using UnityEngine;

[RequireComponent(typeof(WalkState))]
[RequireComponent(typeof(DashState))]
public class IdleState : State
{
    private ItemCarryController _controller;
    private Rigidbody _rb;

    private void Awake()
    {
        _controller = gameObject.Find<ItemCarryController>();
        _rb = gameObject.Find<Rigidbody>();
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (_controller.TryPickupPart())
        {
            return gameObject.Find<PickupState>();
        }

        if (_rb.velocity.magnitude > 0.1f)
        {
            return gameObject.Find<WalkState>();
        }

        return this;
    }
}
