using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float despawnTimer = 7.0f;

    Rigidbody2D rigidbody2d;
    

    // Awake is called when a GameObject is initialized, even before start function
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        despawnTimer -= Time.deltaTime;
        // destroy projectile after the distance is greater than 100f from center of game world
        if(despawnTimer < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        // when AddForce is applied to Rigidbody component the physics engine will apply that force and direction to move the Projectile GameObject every frame.
        rigidbody2d.AddForce(direction * force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Fix();
        }
        Debug.Log("Projectile collision with " + other.gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
