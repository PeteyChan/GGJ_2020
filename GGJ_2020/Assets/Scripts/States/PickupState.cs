using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(DropState))]
public class PickupState : State
{
    [SerializeField] private float _timeToPickupItem;

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeToPickupItem)
        {
            return GetComponent<CarryIdleState>();
        }

        return this;
    }
}
