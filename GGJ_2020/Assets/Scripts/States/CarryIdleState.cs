using UnityEngine;

[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(PickupState))]
[RequireComponent(typeof(DropState))]
public class CarryIdleState : State
{
    //private ItemCarryController _controller;
    Player player;
    private Rigidbody _rb;

    public AnimationClip clip;
    Animate Animate;

    private void Awake()
    {
        gameObject.TryFind(out player);
        _rb = gameObject.Find<Rigidbody>();
    }

    protected override void OnEnter()
    {
        gameObject.Find<Animate>().Play(clip, .1f, 1);
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (player.HeldPart == null)
            return null;

        _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, stateTime * 5);
        
        if (player.GamePad.LeftStick.Tilt() > .1f)
        {
            return GetComponent<CarryWalkState>();
        }

        if (player.GamePad.GetButton(GamePad.Buttons.face_right).wasPressed)
        {
            player.NearbyParts.Remove(player.HeldPart);
            player.HeldPart.Holder = null;
            player.HeldPart = null;
        }

        return this;
    }
}
