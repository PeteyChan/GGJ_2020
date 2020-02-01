using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(PickupState))]
public class DropState : State
{
    [SerializeField] private float _timeToDropItem;

    private ItemCarryController _controller;

    private void Awake()
    {
        _controller = gameObject.Find<ItemCarryController>();
    }

    protected override void OnEnter()
    {
        _controller.BeginDropPart();
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeToDropItem)
        {
            return gameObject.Find<IdleState>();
        }

        return this;
    }

    protected override void OnExit()
    {
        _controller.EndDropPart();
    }
}
