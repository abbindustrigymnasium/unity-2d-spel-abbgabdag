using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy1Controller : MonoBehaviour
{
    [SerializeField] private LayerMask groundCheck;

    //Enemy stats
    private int maxHealth;
    private int currentHealth;
    private float cooldownTimer;
    [SerializeField] private float attackDelay;
    [SerializeField] private int playerDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float colDistance;

    //Enemy animations

    public Animator anim;
    private float destroyDelay;
    public string currentAnim;

    //conditions
    private bool facingRight = true;
    [SerializeField] private BoxCollider2D boxCol;
    [SerializeField] private LayerMask playerLayer;

    private EnemyAI enemyPatrol;
    private SamuraiController playerhealth;



    private void Awake()
    {

        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyAI>();

        maxHealth = 100;
        currentHealth = maxHealth;
        attackDelay = 1.5f;
        playerDamage = 15;
        attackRange = 2.4f;
        colDistance = 0.65f;

        destroyDelay = 2.5f;
        cooldownTimer = Mathf.Infinity;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (PlayerInSight())
        {

            if (cooldownTimer >= attackDelay)
            {
                cooldownTimer = 0f;
                anim.SetTrigger("MeleeAttack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }


    //Other functions

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCol.bounds.center + transform.right * attackRange * transform.localScale.x * colDistance, new Vector3(boxCol.bounds.size.x * attackRange, boxCol.bounds.size.y, boxCol.bounds.size.z), 0f, Vector2.left, 0f, playerLayer);

        if (hit.collider != null)
        {
            playerhealth = hit.transform.GetComponent<SamuraiController>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCol.bounds.center + transform.right * attackRange * transform.localScale.x * colDistance, new Vector3(boxCol.bounds.size.x * attackRange, boxCol.bounds.size.y, boxCol.bounds.size.z));
    }

    public void EnemyTakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("TakeHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerhealth.PlayerTakeDamage(playerDamage);
        }

    }

    void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        PlayAnimDestroy();
        Destroy(gameObject, destroyDelay);
    }

    void PlayAnimDestroy()
    {
        anim.SetBool("IsDead", true);
    }
    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }
}
