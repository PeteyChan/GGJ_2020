using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(PickupState))]
[RequireComponent(typeof(DropState))]
public class CarryWalkState : State
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
        if (_controller.TryDropPart())
        {
            return GetComponent<DropState>();
        }

        if (_rb.velocity.magnitude < 0.1f)
        {
            return GetComponent<CarryIdleState>();
        }

        return this;
    }
}
