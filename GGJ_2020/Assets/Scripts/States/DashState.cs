using UnityEngine;

[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(WalkState))]
public class DashState : State
{
    [SerializeField] private float _dashLengthInSeconds;
    public float dashSpeed = 10;

    Player player;
    Rigidbody Rigidbody;
    Animate animate;

    public AnimationClip clip;

    private void Awake()
    {
        gameObject.TryFind(out player);
        gameObject.TryFind(out Rigidbody);
        gameObject.TryFind(out animate);
    }
    
    protected override void OnEnter()
    {
        animate.Speed = 6;
        animate.Play(clip, .1f);
    }

    protected override void OnExit()
    {
        animate.Speed = 1;
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        var vel = Vector3.Lerp(Rigidbody.velocity, animate.transform.forward * dashSpeed, Time.deltaTime*4f);

        Rigidbody.AddForce(vel, ForceMode.VelocityChange);
        
        if (stateTime > _dashLengthInSeconds)
        {
            return GetComponent<WalkState>();
        }

        return this;
    }
}
