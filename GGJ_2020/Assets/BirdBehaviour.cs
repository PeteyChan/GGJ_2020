using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    static int Count;

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
        var rot = transform.eulerAngles;
        rot.y = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(rot);

        speed = Random.Range(2, 6);
    }

    bool moving;
    float random;
    public float timer;
    float speed;

    static float lastSpawn;
    Quaternion targetRotation;
    // Update is called once per frame
    void Update()
    {
        if (Count < 50 && lastSpawn + 1f < Time.time)
        {
            Debug.Log("Spawn");
            lastSpawn = Time.time;
            if (Count % 2 == 1)
            {
                var dir = Vector3.left + Vector3.forward * Random.Range(-1f, 1f);
                Instantiate(Resources.Load("Bird"), new Vector3(GameSettings.MapSize.x, 3, Random.Range(0, GameSettings.MapSize.y)), Quaternion.LookRotation(dir));
            }
            else
            {
                var dir = Vector3.right + Vector3.forward * Random.Range(-1f, 1f);
                Instantiate(Resources.Load("Bird"), new Vector3(-GameSettings.MapSize.x, 3, Random.Range(0, GameSettings.MapSize.y)), Quaternion.LookRotation(dir));
            }
        }

        transform.position += transform.forward * Time.deltaTime * speed;
        var pos = new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        var x = transform.position.x;
        var z = transform.position.z;

        if (x < -GameSettings.MapSize.x - 10 || x > GameSettings.MapSize.x + 10)
            Destroy(gameObject);
        if (z < -10 || z > GameSettings.MapSize.y + 10)
            Destroy(gameObject);
    }
}
