using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

public class SamuraiController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask enemyLayers;

    private bool facingRight = true;
    private bool isJumping = false;

    //Samurai Stats
    private int maxHealth;
    private int currentHealth;
    
    [SerializeField] private int jumpForce;
    [SerializeField] float moveSpeed;

    //combat system

    public int attackDmg;
    private float attackDelay;
    private float attackRate;
    private float nextAttackTime;
    private bool isAttackPressed;
    private bool isAttacking;


    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;

    //Samurai animations
    [SerializeField] public Animator anim;
    public string currentAnim;
    private string playerIdle = "Samurai_Idle";
    private string playerRun = "Samurai_Run";
    private string playerFall = "Samurai_Fall";
    private string playerJump = "Samurai_Jump";
    private string playerAttack_1 = "Samurai_Attack1";
    private float destroyDelay;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        anim = gameObject.GetComponent<Animator>();

        jumpForce = 16;
        attackDmg = 40;
        moveSpeed = 500;
        attackRange = 0.9f;
        attackRate = 2f;
        nextAttackTime = 0;
        destroyDelay = 2.5f;

        maxHealth = 100;
        currentHealth = maxHealth;
    }


    private void Update()
    {
        //prevent spam attack
        if(Time.time >= nextAttackTime)
        {
            //check if samurai is attacking
            if (Input.GetMouseButtonDown(0))
            {
                isAttacking = true;
                anim.SetTrigger("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        //Samurai jump
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpForce;

        }

        //Change to jump animations
        if (!IsGrounded())
        {
            if (isJumping)
            {
                ChangeAnimationState(playerJump);
                if (rb.velocity.y <= 0)
                {
                    isJumping = false;
                }
            }
            else if (rb.velocity.y < 0)
            {
                ChangeAnimationState(playerFall);
            }
        }

        
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    public void ChangeAnimationState(string newAnimation)
    {
        if (newAnimation == currentAnim)
            return;

        anim.Play(newAnimation);
        currentAnim = newAnimation;
    }

    public bool IsAnimPlaying(Animator anim, string stateName)
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FixedUpdate()
    {

        //Samurai movement

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            if (IsGrounded() && !isAttackPressed)
                ChangeAnimationState(playerRun);
            if (facingRight)
                Flip();
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector2(+moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
                if (IsGrounded() && !isAttackPressed)
                    ChangeAnimationState(playerRun);
                if (!facingRight)
                    Flip();
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if (IsGrounded() && !isAttackPressed)
                    ChangeAnimationState(playerIdle);
            }
        }
    }

    //Check if samurai is touching ground
    private bool IsGrounded()
    {
        float extraHeightTest = .1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, boxCollider2D.bounds.extents.y + extraHeightTest, platformLayer);
        Color rayColor;
        if (boxCastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightTest), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightTest), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y), Vector2.right * (boxCollider2D.bounds.extents.x), rayColor);
        return boxCastHit.collider != null;
    }

    void AttackComplete()
    {
        isAttackPressed = false;
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy1Controller>().EnemyTakeDamage(attackDmg);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void PlayerTakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("TakeHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PlayAnimDestroy();
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, destroyDelay);
    }

    private void PlayAnimDestroy()
    {
        anim.SetBool("IsDead", true);
    }
}
