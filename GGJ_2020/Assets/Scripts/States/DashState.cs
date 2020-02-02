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

    Vector3 enterVel;

    public Vector3 enterPOs;

    protected override void OnEnter()
    {
        player.DoodadCollision = null;
        player.PlayerCollision = null;

        animate.Speed = 6;
        animate.Play(clip, .1f);

        enterPOs = Rigidbody.position;
        speed = dashSpeed;
    }

    protected override void OnExit()
    {
        animate.Speed = 1;
    }

    float speed;
    float resetDash;
    
    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (player.GamePad.GetButton(GamePad.Buttons.face_down).wasPressed)
        {
            speed += dashSpeed;
            resetDash = stateTime;
        }
        var dir = animate.transform.forward;
        if (player.GamePad.LeftStick.magnitude > .1f)
        {
            var inputdir = player.GamePad.LeftStick;

            float control = 1;
            dir = new Vector3(Mathf.Lerp(dir.x, inputdir.x, control * deltaTime), 0, Mathf.Lerp(dir.z, inputdir.y, control * deltaTime));
        }
        animate.transform.forward = dir;

        Rigidbody.velocity = Vector3.Lerp(Rigidbody.velocity, animate.transform.forward * speed, deltaTime * 5f);

        if (player.DoodadCollision)
            return gameObject.Find<KnockedDownState>();

        if (player.PlayerCollision)
        {
            var knockDown = player.PlayerCollision.gameObject.Find<KnockedDownState>();
            knockDown.knockedDownForce = (Rigidbody.position - enterPOs)/stateTime;
            player.PlayerCollision.gameObject.Find<StateMachine>().ChangeState(knockDown);
            return gameObject.Find<KnockedDownState>();
        }

        if (stateTime > (_dashLengthInSeconds + resetDash))
        {
            return null;
        }
        return this;
    }
}
