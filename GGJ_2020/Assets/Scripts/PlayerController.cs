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

    public bool TryAddHeldPartToShip()
    {
        if (_player.NearbyShip != null && _player.HeldPart != null)//&& ship.Team == GameSettings.GetPlayerInfo(_player.PlayerNumber).Team)
        {
            _player.NearbyShip.AddPart(_player.HeldPart);
            _player.HeldPart = null;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShipPart part = collision.gameObject.GetComponent<ShipPart>();

        if (part != null && _player.NearbyPart != part)
        {
            _player.NearbyPart = part;
            return;
        }

        BrokenShip ship = collision.gameObject.GetComponent<BrokenShip>();

        if (ship != null && _player.NearbyShip != ship)
        {
            _player.NearbyShip = ship;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        ShipPart part = collision.gameObject.GetComponent<ShipPart>();

        if (part != null)
        {
            if (_player.NearbyPart == part)
            {
                _player.NearbyPart = null;
            }

            return;
        }

        BrokenShip ship = collision.gameObject.GetComponent<BrokenShip>();

        if (ship != null)
        {
            if (_player.NearbyShip == ship)
            {
                _player.NearbyShip = null;
            }

            return;
        }
    }
}
