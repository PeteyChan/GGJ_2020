using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBehaviour : MonoBehaviour
{
    public AnimationClip idle;
    public AnimationClip walk;
    Animate animate;

    public static int Count;

    private void Awake()
    {
        Count++;
    }

    private void OnDestroy()
    {
        Count--;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.TryFind(out animate);
        animate.Play(idle);
        random = Random.Range(2, 4);

        var rot = transform.eulerAngles;
        rot.y = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(rot);
    }

    bool moving;
    float random;
    public float timer;

    Quaternion targetRotation;
    // Update is called once per frame
    void Update()
    {
        if (CrabBehaviour.Count < 100)
        {
            var pos = new Vector3(Random.Range(-GameSettings.MapSize.x, GameSettings.MapSize.x), 0, Random.Range(0, GameSettings.MapSize.y));

            Instantiate(Resources.Load<GameObject>("Crab"), pos, Quaternion.identity);
        }

        timer += Time.deltaTime;

        if (timer > random)
        {
            moving = !moving;
            random = Random.Range(2, 4);

            if (moving)
                animate.Play(walk);
            else animate.Play(idle);
            timer = 0;

            var rot = transform.eulerAngles;
            rot.y = Random.Range(0, 360);
            targetRotation = Quaternion.Euler(rot);
        }

        if (moving)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
            transform.position += transform.forward* Time.deltaTime;
            var pos = new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

            var x = transform.position.x;
            var z = transform.position.z;

            if (x < -GameSettings.MapSize.x || x > GameSettings.MapSize.x)
                Destroy(gameObject);
            if (z < 0 || z > GameSettings.MapSize.y)
                Destroy(gameObject);
        }
    }
}
