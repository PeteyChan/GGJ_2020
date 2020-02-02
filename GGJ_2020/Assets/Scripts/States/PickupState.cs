using UnityEngine;

[RequireComponent(typeof(CarryIdleState))]
[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(DropState))]
public class PickupState : State
{
    public AnimationClip clip;

    Animate Animate;
    public float pickupTime;
    public float heldTime;

    Player player;
    Rigidbody Rigidbody;

    private void Awake()
    {
        gameObject.TryFind(out player);
        gameObject.TryFind(out Animate);
        gameObject.TryFind(out Rigidbody);
    }

    Vector3 partPos;
    protected override void OnEnter()
    {
        var part = player.GetPart();

        Animate.Play(clip);
        AudioSource.PlayClipAtPoint(GameSounds.Instance.Lifting, FindObjectOfType<AudioListener>().transform.position);
        partPos = part.transform.position;
        part.Holder = player;
        player.HeldPart = part;
        part.enabled = false;
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        Rigidbody.velocity = Vector3.Lerp(Rigidbody.velocity, Vector3.zero, stateTime * 5f);
        var target = Rigidbody.position - partPos;
        target.y = 0;
        target = target.normalized;
        Animate.transform.rotation = Quaternion.Lerp(Animate.transform.rotation, Quaternion.LookRotation(target), deltaTime * 10f);

        if (stateTime > pickupTime)
        {
            var dif = heldTime - pickupTime;
            var time = stateTime - pickupTime;
            
            player.HeldPart.transform.position = Vector3.Lerp(partPos, Rigidbody.position + Vector3.up, time/dif);
        }

        if (stateTime > heldTime)
        {
            return gameObject.Find<CarryIdleState>();
        }
        return this;
    }

    protected override void OnExit()
    {
        player.HeldPart.enabled = true;
    }
}
