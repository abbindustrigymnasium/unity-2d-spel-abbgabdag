using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;


    [Header("Enemy")]
    [SerializeField] private Transform enemy;


    [Header("Movement params")]
    [SerializeField] private float moveSpeed;
    private Vector3 initScale;
    private bool movingLeft;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        initScale = enemy.localScale;
        moveSpeed = 4;
    }
    private void OnDisable()
    {
        anim.SetBool("Moving", false);
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDir(-1);
            }
            else
            {
                DirChange();
            }

        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDir(1);
            }
            else
            {
                DirChange();
            }
        }
    }
    private void MoveInDir(int dir)
    {
        anim.SetBool("Moving", true);
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * dir * moveSpeed, enemy.position.y, enemy.position.z);
    }

    private void DirChange()
    {
        anim.SetBool("Moving", false);
        movingLeft = !movingLeft;
    }

}
