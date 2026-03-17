using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 3.0f;
    // Controls how long the player remain invvincible after taking damage
    public float timeInvincible = 2.0f;
    public float timeBetweenRegen = 1.0f;
    // bool store if the player is invincible or not
    bool isInvincible;
    bool isRegenable;
    // Store how much time is remaining until player not invincible
    float damageCooldown;
    float regenCooldown;
    public int maxHealth = 5;
    int currentHealth;
    // We call this because we do not want to make currentHealth to public access.
    // Instead we use this method to create a "Read-only mode" to avoid less bug in the future.
    public int health { get {  return currentHealth; } } 

    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;
    public GameObject projectilePrefab;
    public event Action OnTalkedToNPC;
    public int launchForce = 300;

    // Sound Effects
    public AudioClip MoveSound;
    public AudioClip LaunchSound;
    public AudioClip HitSound;

    public NonPlayerCharacter currentInteractingNPC { get; private set; }
    private NonPlayerCharacter lastNonPlayerCharacter;

    Animator animator;
    AudioSource audioSource;

    // new creates a space in memory to store a new variable instance
    Vector2 moveDirection = new Vector2(1, 0);
    Rigidbody2D rigidbody2d;
    Vector2 move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // MoveAction listens to the input player key in. ReadValue get the value of the input, <> contains the value type that will be read
        move = MoveAction.ReadValue<Vector2>();

        // Mathf.Approximately is to counter the almost zero problem, because during an operation the value return could be greater than 0.0f (i.e. 0.000001f)
        if (!Mathf.Approximately(move.x,0.0f) || !Mathf.Approximately(move.y,0.0f))
        {   

            // Set to the current movement values, when the character is not moving it remains the last movement value provided.
            moveDirection.Set(move.x, move.y);
            // Call normalize, which sets its length to 1 but keep the direction.
            moveDirection.Normalize();
            if (!audioSource.isPlaying)
            {
                PlaySound(MoveSound);
            }
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        // length of 0 to 1, 0 for stationary and 1 for moving. because we have normalized the length
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if(damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
        if(isRegenable)
        {
            regenCooldown -= Time.deltaTime;
            if (regenCooldown < 0)
            {
                isRegenable = false;
            }
        }
        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }

        // Four argument passed through this raycast.
        // First argument passes the position of the Player.
        // Second argument passes the direction that the player character is looking
        // Third argument is the maximum distance of the ray.
        // Final argument defines a layer mask, which is a way to only check within specified layers. In this case the NPC layer.
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if(hit.collider != null)
        {
            // in this line we will detect if the component is a NonPlayerCharacter. if yes we store it as npc
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            npc.dialogueBubble.SetActive(true);
            // in this line we will record down who we just talked to, so after we exit the area we will disable the bubble dialogue
            lastNonPlayerCharacter = npc;
            currentInteractingNPC = npc;
            FindFriend(hit, npc);
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
                currentInteractingNPC = null;
            }
        }
    }

    void FixedUpdate()
    {
        // Time.deltaTime is a variable that Unity uses to store the tiem it takes for a frame to be rendered
        Vector2 position = (Vector2)transform.position + move * movementSpeed * Time.deltaTime;
        
        // This sets the player character's position moves the GameObject based on the user's input.
        // But will stop if the player character hits a GameObject that also has a collider.
        rigidbody2d.MovePosition(position);
    }

    // int amount here is a parameter that will be passed to the function,
    // which will be use to change the currentHealth variable
    public void ChangeHealth (int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(HitSound);
        }
        if (amount > 0) {
            if (isRegenable)
            {
                return;
            }
            isRegenable = true;
            regenCooldown = timeBetweenRegen;
        }
        // currentHealth cannot be set to a value over maxHealth or below 0
        // Mathf.Clamp here help to fix the range of possible health values.
        // First value is the value that need to be restricted (currentHealth + amount)
        // Second value is the minimum allowed value. Third value is the maximum allowed value.
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        // we passes the ratio of current health and maxhealth to calculate the size of the healthbar
        // (float)maxHealth is to resolve the division problem of 2/4 will get 0. if changed it to float 2/4.0 will give 0.5 instead
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        // Instantiate Create or Duplicate a copy you have already made.
        // projectilePrefab tells Instantiate to make a copy of this specific prefab
        // rigidbody2d.position + Vector2.up * 0.5f tells the location of the object to spawn at which is right where the player was standing and Move it up so it was from the middle of the player.
        // Quaternion.identity is to not let the object rotate.
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        // Gets reference from the projectile script
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        // Calls Launch function from projectile passing it the direction and the force value
        projectile.Launch(moveDirection, launchForce);
        PlaySound(LaunchSound);
        animator.SetTrigger("Launch");
    }

    void FindFriend(RaycastHit2D hit, NonPlayerCharacter npc)
    {
        if (TalkAction.WasPressedThisFrame())
        {
            //UIHandler.instance.DisplayDialogue(npc.npcMessage);
            OnTalkedToNPC?.Invoke();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
