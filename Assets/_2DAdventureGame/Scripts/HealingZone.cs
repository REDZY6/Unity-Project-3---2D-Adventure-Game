using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public int healthIncrease = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.ChangeHealth(healthIncrease);
        }
    }
}
