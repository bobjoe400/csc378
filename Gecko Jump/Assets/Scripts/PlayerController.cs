using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerSettings settings;
    public Rigidbody2D rb;
    public LayerMask platformLayer;

    public bool showDirectionVectors = true;
    public bool showCollisionRays = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnimParam = "Idle";
    [SerializeField] private string movingAnimParam = "Moving";
    [SerializeField] private string attackAnimParam = "Attack";
    [SerializeField] private float attackAnimationDuration = 0.3f; // Match this with your animation 

    [Header("Player Effects")]
    [SerializeField] private ParticleSystem jumpingParticleSystem; // Assign a ParticleSystem in scene

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;

    // Add to your PlayerMovement class
    [Header("Attack Settings")]
    [SerializeField] public GameObject attackHitbox;
    [SerializeField] public Vector2 attackHitboxOffset = new Vector2(0.5f, 0); // Default offset
    private Vector2 initialHitboxOffset; // Store the initial relative position
    private bool isAttacking = false;

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer characterSprite;
    
    private float horizontal;
    private bool isFacingRight = true;
    private bool isUpsideDown = false;
    private bool isWallJumping = false;
    private bool wasGrounded = false;
    private bool horizontalInputReleased = false;
    
 // RENAMED direction vectors with correct semantic meaning
    [Header("Direction Vectors")]
    [SerializeField] private Vector2 pillLeft;    // transform.up (points to player's RIGHT)
    [SerializeField] private Vector2 pillDown; // transform.right (points to player's DOWN)
    
    [SerializeField] private Vector2 visualUp;    // Corrected vector pointing UPWARD on screen
    [SerializeField] public Vector2 visualRight; // Corrected vector pointing RIGHT/LEFT based on facing
    [SerializeField] private float realAngle; // Angle of the pill's orientation
    [SerializeField] private float visualAngle; // Angle of the visual up vector


    // Add the Serializable attribute to make ContactInfo visible in the Inspector
    [System.Serializable]
    public class ContactInfo
    {
        [Header("Contact State")]
        public bool isInContact;      // Whether we're in contact with something
        public bool isGround;         // Is this a ground-like surface (based on angle)
        public bool isWall;           // Is this a wall-like surface (based on angle)
        
        [Header("Contact Data")]
        public Vector2 contactNormal; // Normal of the contact surface
        public Vector2 contactPoint;  // Point of contact
        public float contactDistance; // Distance to the contact
        public float dotProduct;      // Dot product with up (could add this for debugging)
        
        public ContactInfo()
        {
            Reset();
        }
        
        public void Reset()
        {
            isInContact = false;
            contactNormal = Vector2.zero;
            contactPoint = Vector2.zero;
            contactDistance = float.MaxValue;
            isGround = false;
            isWall = false;
            dotProduct = 0f;
        }
    }

    // Then change your private fields to [SerializeField] to make them visible
    [Header("Contact Information")]
    [SerializeField] private ContactInfo leftContact = new ContactInfo();
    [SerializeField] private ContactInfo rightContact = new ContactInfo();
    [SerializeField] private ContactInfo topContact = new ContactInfo();
    [SerializeField] private ContactInfo bottomContact = new ContactInfo();
        
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Store the initial hitbox position relative to the player
        if (attackHitbox != null)
        {
            initialHitboxOffset = attackHitbox.transform.localPosition;
            attackHitbox.SetActive(false); // Ensure it starts inactive
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        if (characterSprite == null)
            characterSprite = GetComponentInChildren<SpriteRenderer>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Initialize direction vectors
        UpdateDirectionVectors();
    }
    
    void Update()
    {
        // Update direction vectors first
        UpdateDirectionVectors();

        UpdateHitboxPosition();
        
        // Apply horizontal movement
        if (!isWallJumping || horizontalInputReleased)
        {
            rb.linearVelocity = new Vector2(horizontal * settings.speed, rb.linearVelocity.y);
        }

        if (isWallJumping && horizontal == 0)
        {
            horizontalInputReleased = true;
        }
    
        // Check for contacts
        CheckContacts();

        // Update direction vectors based on current state
        UpdateDirectionVectors();

        // Handle flipping with our new logic
        HandleOrientation();
        
        bool isGroundedNow = IsGrounded();
        if (isGroundedNow && !wasGrounded && landSound != null)
        {
            PlaySound(landSound);
        }
        
        wasGrounded = isGroundedNow;

        animator.SetBool("Grounded", isGroundedNow);

        // Update animation state
        UpdateAnimationState();

        // Draw debug rays
        if (showDirectionVectors)
        {
            DrawDebugDirections();
        }
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        // Set movement animation
        bool isMoving = Mathf.Abs(horizontal) > 0.1f;
        animator.SetBool(movingAnimParam, isMoving && !isAttacking);
        
        // Idle is handled by transitions in the Animator
        animator.SetBool(idleAnimParam, !isMoving && !isAttacking);
        
        // Attack is triggered via the Attack method
    }

    private void UpdateDirectionVectors()
    {
        // Get the pill's actual transform directions
        pillLeft = transform.up;
        pillDown = transform.right;
        
        // Get a vector pointing upward relative to screen
        Vector2 approximateUp = -pillDown;
        
        // Calculate the raw angle between world up and the pill's "up"
        realAngle = Vector2.SignedAngle(Vector2.up, approximateUp);
        
        // Adjust angle to ensure it represents the shortest rotation to get the pill upright
        if (realAngle > 90f)
        {
            visualAngle = 180f - realAngle;
        } 
        else if (realAngle < -90f)
        {
            visualAngle = -180f - realAngle;
        }
        else
        {
            visualAngle = realAngle;
        }
        
        // Create the visual up vector that's always pointing generally upward
        visualUp = Quaternion.Euler(0, 0, visualAngle) * Vector2.up;
        
        // IMPORTANT: Visual right should follow actual movement direction
        // Not just based on the pill's rotation
        visualRight = Quaternion.Euler(0, 0, visualAngle) * 
                    (horizontal > 0 ? Vector2.right : 
                    (horizontal < 0 ? Vector2.left : 
                    (isFacingRight ? Vector2.right : Vector2.left)));
    }
    
    // Draw debug rays to visualize the vectors
    // Draw debug rays to visualize the vectors with clear color coding
    private void DrawDebugDirections()
    {
        Vector3 localOffset = new Vector3(0.1f, 0.1f, 0);
        Vector3 worldOffset = new Vector3(0.2f, 0.2f, 0);

        // Pill's actual local axes
        Debug.DrawRay(transform.position + localOffset, pillLeft * 0.5f, Color.magenta);    // Points to player's RIGHT
        Debug.DrawRay(transform.position + localOffset, pillDown * 0.5f, Color.yellow);  // Points to player's DOWN
        
        // Corrected visual directions
        Debug.DrawRay(transform.position, visualUp * 0.5f, Color.green);         // Points UPWARD on screen
        Debug.DrawRay(transform.position, visualRight * 0.5f, Color.red);        // Points RIGHT/LEFT based on facing
        
        // World directions for reference
        Debug.DrawRay(transform.position + worldOffset, Vector2.up * 0.3f, new Color(0, 0.5f, 0));      // World up
        Debug.DrawRay(transform.position + worldOffset, Vector2.right * 0.3f, new Color(0.5f, 0, 0));   // World right
    }

    private void CheckContacts()
    {
        // Reset all contact info
        leftContact.Reset();
        rightContact.Reset();
        topContact.Reset();
        bottomContact.Reset();
        
        // Get collider size
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        float width = capsule.size.y;
        float height = capsule.size.x;
        
        // Get ray settings from settings object
        int rayCount = settings.numberOfRays;
        float edgeInset = settings.edgeInsetFactor;

        float sizeRatio = transform.localScale.y / transform.localScale.x;
        
        // Distance from center along each axis for ray starting positions
        float longEdgeOffset = height * settings.longEdgeOffsetFactor * 0.5f;
        float shortEdgeOffset = width * settings.shortEdgeOffsetFactor * sizeRatio * 0.5f;
        
        // Calculate the distribution widths
        float leftRightDistWidth = width * settings.longEdgeWidthFactor;
        float topBottomDistHeight = height * settings.shortEdgeWidthFactor;
        
        // Calculate ray distribution ranges with inset
        float topBottomStartX = -(topBottomDistHeight * 0.5f) + (height * edgeInset);
        float topBottomEndX = (topBottomDistHeight * 0.5f) - (height * edgeInset);
        
        float leftRightStartY = -(leftRightDistWidth * 0.5f) + (width * edgeInset);
        float leftRightEndY = (leftRightDistWidth * 0.5f) - (width * edgeInset);
        
        // IMPORTANT: We still use the pill's actual axes for ray casting
        // because the physics follow the pill's orientation
        
        // TOP edge rays
        for (int i = 0; i < rayCount; i++)
        {
            float t = (i + 1) / (float)(rayCount + 1);
            float yOffset = Mathf.Lerp(leftRightStartY, leftRightEndY, t);
            Vector2 startPos = (Vector2)transform.position - pillDown * longEdgeOffset + pillLeft * yOffset;
            CastRay(startPos, -pillDown, topContact);
        }
        
        // BOTTOM edge rays
        for (int i = 0; i < rayCount; i++)
        {
            float t = (i + 1) / (float)(rayCount + 1);
            float yOffset = Mathf.Lerp(leftRightStartY, leftRightEndY, t);
            Vector2 startPos = (Vector2)transform.position + pillDown * longEdgeOffset + pillLeft * yOffset;
            CastRay(startPos, pillDown, bottomContact);
        }
        
        // LEFT edge rays
        for (int i = 0; i < rayCount; i++)
        {
            float t = (i + 1) / (float)(rayCount + 1);
            float xOffset = Mathf.Lerp(topBottomStartX, topBottomEndX, t);
            Vector2 startPos = (Vector2)transform.position + pillLeft * shortEdgeOffset + pillDown * xOffset;
            CastRay(startPos, pillLeft, leftContact);
        }
        
        // RIGHT edge rays
        for (int i = 0; i < rayCount; i++)
        {
            float t = (i + 1) / (float)(rayCount + 1);
            float xOffset = Mathf.Lerp(topBottomStartX, topBottomEndX, t);
            Vector2 startPos = (Vector2)transform.position - pillLeft * shortEdgeOffset + pillDown * xOffset;
            CastRay(startPos, -pillLeft, rightContact);
        }
    }
    
    private void CastRay(Vector2 start, Vector2 direction, ContactInfo contactInfo)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction, settings.rayLength, platformLayer);
        
        if (showCollisionRays)
        {
            Color rayColor = hit.collider != null ? Color.green : Color.red;
            Debug.DrawRay(start, direction * settings.rayLength, rayColor);
        }
        
        if (hit.collider != null && hit.distance < contactInfo.contactDistance)
        {
            contactInfo.isInContact = true;
            contactInfo.contactNormal = hit.normal;
            contactInfo.contactPoint = hit.point;
            contactInfo.contactDistance = hit.distance;
            
            // Use world up for consistent ground detection
            float upDot = Vector2.Dot(hit.normal, Vector2.up);
            contactInfo.isGround = upDot > settings.groundAngleThreshold;
            contactInfo.isWall = Mathf.Abs(upDot) < settings.wallAngleThreshold;
        }
    }
    
    private void HandleOrientation()
    {
        // Calculate the angle difference
        float angleDifference = Mathf.Abs(realAngle - visualAngle);

        // Add hysteresis to prevent flickering near threshold
        // Only change vertical orientation when clearly beyond threshold
        bool shouldFlipVertically = isUpsideDown; // Start with current state
        
        // Only change vertical flipping state when we're clearly beyond the threshold
        if (angleDifference > 110f && !isUpsideDown)
        {
            shouldFlipVertically = true;
        }
        else if (angleDifference < 70f && isUpsideDown)
        {
            shouldFlipVertically = false;
        }
        
        // Determine horizontal orientation based on movement and visual vectors
        bool shouldFlipHorizontally = !isFacingRight; // Start with current state
        
        // Only change horizontal orientation on clear input
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Base horizontal orientation on movement direction
            shouldFlipHorizontally = horizontal < 0;
            
            // When upside down on ground, invert horizontal orientation
            if (shouldFlipVertically)
            {
                shouldFlipHorizontally = !shouldFlipHorizontally;
            }
        }
        
        // Special case for wall contact - completely override orientation
        if (IsOnWall())
        {
            // Get the wall normal and contact info
            Vector2 wallNormal = Vector2.zero;
            bool isTopContact = topContact.isWall;
            bool isBottomContact = bottomContact.isWall;
            
            if (isTopContact) wallNormal = topContact.contactNormal;
            else if (isBottomContact) wallNormal = bottomContact.contactNormal;
            
            // Determine if this is a left or right wall
            bool isRightWall = Vector2.Dot(wallNormal, Vector2.left) > 0;
            
            // COMPLETELY REVISED WALL ORIENTATION LOGIC
            // We know which edge is touching the wall (top or bottom)
            // and which way the wall is facing (left or right)
            
            // 1. Set horizontal orientation based on wall direction
            // Character should face away from wall
            shouldFlipHorizontally = (!isRightWall && isBottomContact) || (isRightWall && isTopContact); // On right wall, face left
            
            // 2. For vertical orientation, we need to check which edge is touching
            if (isTopContact)
            {
                // Top edge touching means character needs to be upside down
                shouldFlipVertically = true;
            }
            else if (isBottomContact)
            {
                // Bottom edge touching means character needs to be right-side up
                shouldFlipVertically = false;
            }
        }

        // Apply orientation to sprite
        if (characterSprite != null)
        {
            characterSprite.flipY = shouldFlipHorizontally;
            characterSprite.flipX = shouldFlipVertically;
        }

        // Apply orientation to the particle system
        if (jumpingParticleSystem != null)
        {
            Vector2 pos = jumpingParticleSystem.transform.localPosition;

            // Flip Y if horizontal orientation changed
            if ((shouldFlipHorizontally && pos.y > 0) || (!shouldFlipHorizontally && pos.y < 0))
                pos.y *= -1;

            // Flip X if vertical orientation changed
            if ((shouldFlipVertically && pos.x > 0) || (!shouldFlipVertically && pos.x < 0))
                pos.x *= -1;

            jumpingParticleSystem.transform.localPosition = pos;
        }

        // Store current facing state for other gameplay logic
        isFacingRight = !shouldFlipHorizontally;
        isUpsideDown = shouldFlipVertically;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        
        if (isWallJumping && horizontalInputReleased && horizontal != 0)
        {
            isWallJumping = false;
        }
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            PerformGroundJump();
        }
        else if (context.performed && IsOnWall())
        {
            PerformWallJump();
        }
        
        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void PerformGroundJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, settings.jumpingPower);

        // Play jump sound
        PlaySound(jumpSound);

        jumpingParticleSystem.Play();
    }
    
    private void PerformWallJump()
    {
        Vector2 wallNormal = GetWallJumpNormal();
        
        if (wallNormal == Vector2.zero)
            return;
            
        rb.linearVelocity = Vector2.zero;
        Vector2 jumpDirection = wallNormal * settings.wallJumpDirectionPower + Vector2.up * settings.wallJumpPower;
        rb.AddForce(jumpDirection, ForceMode2D.Impulse);
        
        isWallJumping = true;
        horizontalInputReleased = false;
        
        // Play jump sound
        PlaySound(jumpSound);

        StartCoroutine(PreventWallStick());
    }
    
    private Vector2 GetWallJumpNormal()
    {   
        if (math.abs(horizontal) == 0)
            return Vector2.zero;

        if (topContact.isWall)
            return topContact.contactNormal;
            
        if (bottomContact.isWall)
            return bottomContact.contactNormal;
            
        return Vector2.zero;
    }
    
    private IEnumerator PreventWallStick()
    {
        yield return new WaitForSeconds(settings.wallJumpCooldown);
        yield return new WaitForSeconds(0.1f);
        isWallJumping = false;
    }
    
    public bool IsGrounded()
    {
        return bottomContact.isGround || topContact.isGround || leftContact.isGround || rightContact.isGround;
    }
    
    public bool IsOnWall()
    {
        return (topContact.isWall || bottomContact.isWall) && !IsGrounded();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Trigger attack animation
        if (animator != null)
        {
            attackHitbox.SetActive(true);
            animator.SetTrigger(attackAnimParam);
        }
        
        // Wait for attack duration
        yield return new WaitForSeconds(attackAnimationDuration);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
        
        isAttacking = false;
    }

    private void UpdateHitboxPosition()
    {
        if (attackHitbox != null)
        {
            // Start with the initial offset
            Vector2 offset = initialHitboxOffset;
            
            // Adjust based on facing direction
            if (!isFacingRight)
            {
                // Mirror the offset when facing left
                offset.y = -offset.y;
            }
            
            // Apply offset and rotational adjustments if necessary
            if (isUpsideDown)
            {
                // When upside down, adjust the offset accordingly
                offset.x = -offset.x;
            }
            
            // Set the local position to maintain proper relative positioning
            attackHitbox.transform.localPosition = offset;
        }
    }

    public void PlayFootstepSound()
    {
        PlaySound(footstepSound);
    }

    public void PlayAttackSound()
    {
        PlaySound(attackSound);
    }

    // Generic sound player method
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }
}