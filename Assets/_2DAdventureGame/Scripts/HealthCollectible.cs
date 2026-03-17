using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int healthRegen = 1;
    public AudioClip collectedClip;

    // other is the parameter that tells you who stepped into your trigger zone
    void OnTriggerEnter2D(Collider2D other)
    { 

        // GetComponent = It looks at the object that was just touched (other) and asks, "Do you have a script attached to you called PlayerController?"It looks at the object that was just touched (other) and asks,
        // "Do you have a script attached to you called PlayerController?"
        // controller = If it finds that script, it saves a reference to it in a new variable named controller.
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            // ChangeHealth(1) sends a data to player, asking them to increase health value by 1
            controller.ChangeHealth(healthRegen);
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }
}
