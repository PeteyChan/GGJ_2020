using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(PickupState))]
[RequireComponent(typeof(DropState))]
public class CarryWalkState : State
{
    //private ItemCarryController _controller;
    Player player;
    private Rigidbody _rb;
    Animate animate;
    public float walkSpeed = 3f;
    public AnimationClip clip;

    private void Awake()
    {
        gameObject.TryFind(out animate);
        gameObject.TryFind(out player);
        _rb = gameObject.Find<Rigidbody>();
    }

    protected override void OnEnter()
    {
        gameObject.Find<Animate>().Play(clip);
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (player.HeldPart == null)
            return null;

        var direction = new Vector3(player.GamePad.LeftStick.x, 0, player.GamePad.LeftStick.y);
        direction = direction.normalized * walkSpeed;

        _rb.velocity = Vector3.Lerp(_rb.velocity, direction, deltaTime * 5f);
        if(direction.magnitude > .1f)
            animate.transform.rotation = Quaternion.Lerp(animate.transform.rotation, Quaternion.LookRotation(direction), deltaTime * 5f);
        
        if (player.GamePad.LeftStick.Tilt() < .1f)
        {
            return GetComponent<CarryIdleState>();
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
