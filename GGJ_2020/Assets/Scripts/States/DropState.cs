using UnityEngine;

public class DropState : State
{
    [SerializeField] private float _timeToDropItem;

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeToDropItem)
        {
            return GetComponent<IdleState>();
        }

        return this;
    }
}
