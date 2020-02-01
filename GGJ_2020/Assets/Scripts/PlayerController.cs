using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private GamePad _pad;
    private Rigidbody _rb;

    [SerializeField] private float _playerSpeed;

    private void Awake()
    {
        _pad = GetComponent<Player>().GamePad;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.velocity = new Vector3(_pad.LeftStick.x, 0, _pad.LeftStick.y) * _playerSpeed;
    }
}
