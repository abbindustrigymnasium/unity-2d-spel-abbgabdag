using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private int jumpForce;
    private Animator animator;
    private string currentAnimation;
    private bool facingRight = true;
    private bool isJumping = false;

    [SerializeField] private LayerMask platformLayerMask;



    //samurai animations
    private string playerIdle = "Samurai_Idle";
    private string playerRun = "Samurai_Run";
    private string playerFall = "Samurai_Fall";
    private string playerJump = "Samurai_Jump";
    private string playerTakeHit = "Samurai_TakeHit";

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        jumpForce = 16;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //Samurai jump
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpForce;
           
        }

        if (!IsGrounded())
        {
            if (isJumping)
            {
                ChangeAnimationState(playerJump);
                if(rb.velocity.y <= 0)
                {
                    isJumping = false;
                }
            }
            else if(rb.velocity.y < 0)
            {
                ChangeAnimationState(playerFall);
            }
        }
    }
    private void ChangeAnimationState(string animation)
    { 
        animator.Play(animation);
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    private void FixedUpdate()
    {

        //Samurai movement
        float moveSpeed = 500;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            if(IsGrounded())
                ChangeAnimationState(playerRun);
            if (facingRight)
                Flip();
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector2(+moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
                if(IsGrounded())
                    ChangeAnimationState(playerRun);
                if (!facingRight)
                    Flip();
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if(IsGrounded())
                    ChangeAnimationState(playerIdle);

            }
        }
    }     
   


    //Check if samurai is touching ground
    private bool IsGrounded()
    {
        float extraHeightTest = .05f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size - new Vector3(0.5f, 0f,0f), 0f, Vector2.down, boxCollider2D.bounds.extents.y + extraHeightTest, platformLayerMask);
        Color rayColor;
        if (boxCastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightTest),rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightTest), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y), Vector2.right * (boxCollider2D.bounds.extents.x), rayColor);
        Debug.Log(boxCastHit.collider);
        return boxCastHit.collider != null;
    }
}
