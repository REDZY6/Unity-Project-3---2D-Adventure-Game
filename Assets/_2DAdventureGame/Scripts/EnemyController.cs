using UnityEngine;
using System;
public class EnemyController : MonoBehaviour
{
    // Public Variable
    public bool vertical;
    public float enemySpeed = 3.0f;
    public float timeChangeDirection = 3.0f;    // How long before change direction
    public int Damage = -1;
    public bool isBroken { get { return broken; } } // Read only function to see how many broken bots
    public event Action OnFixed;
    public AudioClip FixSound;
    public ParticleSystem smokeParticleEffect;

    // Private Variable
    Rigidbody2D rigidbody2d;
    Animator animator;
    AudioSource audioSource;
    float timer;    // track how many time left
    int direction = 1;
    bool broken = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        timer = timeChangeDirection;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = timeChangeDirection;
        }
    }

    void FixedUpdate()
    {     
        Vector2 position = rigidbody2d.position;   

        if(!broken)
        {
            return;
        }

        if (vertical)
        {
            position.y = position.y + enemySpeed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + enemySpeed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);              
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(Damage);
        }
    }

    public void Fix()
    {
        broken = false;
        // Simulated function removves the enemy gameObject from the physics system's simulation for collision
        rigidbody2d.simulated = false;
        audioSource.Stop();
        animator.SetTrigger("Fixed");
        audioSource.PlayOneShot(FixSound);
        smokeParticleEffect.Stop();
        // OnFixed is the event, ? Is anyone listening to this event, Invoke() if so run their code
        OnFixed?.Invoke();
    }

}
