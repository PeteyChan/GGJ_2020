using UnityEngine;

[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(DashState))]
public class WalkState : State
{
    public float walkSpeed = 5f;
    Player player;
    private Rigidbody _rb;

    public AnimationClip clip;
    public float animationSpeed = 2f;

    Animate animate;

    private void Awake()
    {
        gameObject.TryFind(out player);
        _rb = gameObject.Find<Rigidbody>();
        gameObject.TryFind(out animate);
    }

    protected override void OnEnter()
    {
        animate.Play(clip);
        animate.Speed = animationSpeed;
    }

    protected override void OnExit()
    {
        animate.Speed = 1;
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        var direction = new Vector3(player.GamePad.LeftStick.x, 0, player.GamePad.LeftStick.y);
        direction = direction.normalized * walkSpeed;

        _rb.velocity = Vector3.Lerp(_rb.velocity, direction, deltaTime * 3f);
        if (direction.magnitude > .1f)
            animate.transform.rotation = Quaternion.Lerp(animate.transform.rotation, Quaternion.LookRotation(direction), deltaTime * 5f);

        if (player.GamePad.GetButton(GamePad.Buttons.face_left).wasPressed && player.GetPart())
        {
            return gameObject.Find<PickupState>();
        }

        if (player.GamePad.LeftStick.Tilt() < .1f)
        {
            return gameObject.Find<IdleState>();
        }

        if (player.GamePad.GetButton(GamePad.Buttons.face_down).wasPressed)
            return gameObject.Find<DashState>();

        return this;

    }
}
