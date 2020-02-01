using UnityEngine;

[RequireComponent(typeof(WalkState))]
[RequireComponent(typeof(DashState))]
public class IdleState : State
{
    //private ItemCarryController _controller;
    Player player;
    private Rigidbody _rb;

    public AnimationClip clip;

    private void Awake()
    {
        gameObject.TryFind(out player);
        _rb = gameObject.Find<Rigidbody>();
    }

    Vector3 enterVel;

    protected override void OnEnter()
    {
        gameObject.Find<Animate>().Play(clip);
        enterVel = _rb.velocity;
    }


    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        _rb.velocity = Vector3.Lerp(enterVel, Vector3.zero, stateTime * 2f);

        if (player.GamePad.GetButton(GamePad.Buttons.face_left).wasPressed && player.GetPart())
            return gameObject.Find<PickupState>();


        if (player.GamePad.LeftStick.Tilt() > .1f)
        {
            return gameObject.Find<WalkState>();
        }

        if (player.GamePad.GetButton(GamePad.Buttons.face_down).wasPressed)
            return gameObject.Find<DashState>();

        return this;
    }
}
