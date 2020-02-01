using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ItemCarryController))]

public class Player : MonoBehaviour
{
<<<<<<< HEAD
    public GamePad GamePad = new GamePad(0);
=======
    public GamePad GamePad => new GamePad(0);
>>>>>>> a1e2f34855704a56695490854cf19c137bc9c044

    public ShipPart HeldPart { get; set; }

    public ShipPart NearbyPart { get; set; }

    public BrokenShip NearbyShip { get; set; }
}
