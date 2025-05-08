using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerMovement : MonoBehaviour
{
    public PlayerSettings settings;

    public Rigidbody2D rb;
    public LayerMask platformLayer;

    private float horizontal;

    public float Speed 
    {
        get { return settings.speed; }
        set { settings.speed = value; }
    }
    
    public float JumpingPower
    {
        get { return settings.jumpingPower; }
        set { settings.jumpingPower = value; }
    }
    private bool isFacingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontal * Speed, rb.linearVelocity.y);

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        } 
        else if (!isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private bool IsGrounded()
    {
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        // Use a small distance to check below the collider
        float checkDistance = 0.1f;
        
        // Use a box cast slightly smaller than the capsule width
        float boxWidth = capsuleCollider.bounds.size.x * 0.9f;
        Vector2 boxSize = new Vector2(boxWidth, 0.05f);
        
        // Position the box at the bottom of the collider
        Vector2 boxCenter = new Vector2(
            capsuleCollider.bounds.center.x,
            capsuleCollider.bounds.min.y - (boxSize.y / 2)
        );
        
        // Cast the box downward
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.down,
            checkDistance,
            platformLayer
        );
        
        // Visualize the box in Scene view (only visible in Scene view when game is running)
        DrawDebugCube(boxCenter, boxSize, hit.collider != null ? Color.green : Color.red);
        
        return hit.collider != null;
    }

    // Helper method to draw the box for debugging
    private void DrawDebugCube(Vector2 center, Vector2 size, Color color)
    {
        Vector2 halfSize = size / 2;
        
        // Draw the 4 lines of the box
        Debug.DrawLine(new Vector2(center.x - halfSize.x, center.y - halfSize.y), new Vector2(center.x + halfSize.x, center.y - halfSize.y), color);
        Debug.DrawLine(new Vector2(center.x + halfSize.x, center.y - halfSize.y), new Vector2(center.x + halfSize.x, center.y + halfSize.y), color);
        Debug.DrawLine(new Vector2(center.x + halfSize.x, center.y + halfSize.y), new Vector2(center.x - halfSize.x, center.y + halfSize.y), color);
        Debug.DrawLine(new Vector2(center.x - halfSize.x, center.y + halfSize.y), new Vector2(center.x - halfSize.x, center.y - halfSize.y), color);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpingPower);
        }

        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }
}
