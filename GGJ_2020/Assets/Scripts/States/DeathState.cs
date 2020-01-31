using UnityEngine;

public class DeathState : State
{
    [SerializeField] private float _timeInDeathState;

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (stateTime > _timeInDeathState)
        {
            return gameObject.Find<IdleState>();
        }

        return this;
    }
}
