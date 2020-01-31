using UnityEngine;

public class DashState : State
{
    [SerializeField] private float _dashLengthInSeconds;

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _dashLengthInSeconds)
        {
            return GetComponent<IdleState>();
        }

        return this;
    }
}
