using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public GamePad GamePad => new GamePad(1);

    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
}
