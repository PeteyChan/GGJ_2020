using UnityEngine;

[RequireComponent(typeof(Player))]
public class ItemCarryController : MonoBehaviour
{
    private const GamePad.Buttons PICKUP_BUTTON = GamePad.Buttons.face_left;

    private Player _player;
    private PlayerController _controller;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _controller = GetComponent<PlayerController>();
    }

    public bool TryPickupPart()
    {
        return _player.HeldPart == null && _player.GamePad.GetButton(PICKUP_BUTTON).wasPressed && _player.NearbyPart != null;
    }

    public void BeginPickupPart()
    {
        var hinge = _player.NearbyPart.gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = _player.GetComponent<Rigidbody>();
        _controller.AllowMovement(false);
    }

    public void EndPickupPart()
    {
        _player.HeldPart = _player.NearbyPart;
        _controller.AllowMovement(true);
    }

    public bool TryDropPart()
    {
        return _player.HeldPart != null && _player.GamePad.GetButton(PICKUP_BUTTON).wasPressed;
    }

    public void BeginDropPart()
    {
        _controller.AllowMovement(false);
    }

    public void EndDropPart()
    {
        Destroy(_player.HeldPart.GetComponent<HingeJoint>());
        _controller.TryAddHeldPartToShip();
        _player.HeldPart = null;
        _controller.AllowMovement(true);
    }
}
