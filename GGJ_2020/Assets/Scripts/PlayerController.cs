using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;
    private Rigidbody _rb;

    [SerializeField] private float _playerSpeed;

    private bool _canMove = true;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rb = _player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_canMove) 
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        _rb.velocity = new Vector3(_player.GamePad.LeftStick.x, 0, _player.GamePad.LeftStick.y) * _playerSpeed;
    }

    public void AllowMovement(bool state)
    {
        _canMove = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShipPart part = collision.gameObject.GetComponent<ShipPart>();

        if (part != null && _player.NearbyPart != part)
        {
            _player.NearbyPart = part;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        ShipPart part = collision.gameObject.GetComponent<ShipPart>();

        if (part != null && _player.NearbyPart == part)
        {
            _player.NearbyPart = null;
        }
    }
}
