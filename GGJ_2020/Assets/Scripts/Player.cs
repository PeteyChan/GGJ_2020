using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ItemCarryController))]

public class Player : MonoBehaviour
{
    public GamePad GamePad = new GamePad(0);

    public ShipPart HeldPart { get; set; }

    public ShipPart NearbyPart { get; set; }
}
