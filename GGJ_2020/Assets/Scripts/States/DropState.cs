using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(PickupState))]
public class DropState : State
{
    [SerializeField] private float _timeToDropItem;

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeToDropItem)
        {
            return gameObject.Find<IdleState>();
        }

        return this;
    }
}
