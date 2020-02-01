using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(DropState))]
public class PickupState : State
{
    [SerializeField] private float _timeToPickupItem;

    private ItemCarryController _controller;

    private void Awake()
    {
        _controller = gameObject.Find<ItemCarryController>();
    }

    protected override void OnEnter()
    {
        _controller.BeginPickupPart();
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeToPickupItem)
        {
            return gameObject.Find<CarryIdleState>();
        }

        return this;
    }

    protected override void OnExit()
    {
        _controller.EndPickupPart();
    }
}
