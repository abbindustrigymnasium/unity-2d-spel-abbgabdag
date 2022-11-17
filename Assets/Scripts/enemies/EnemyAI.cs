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
    private Transform enemyPos;


    [Header("Movement params")]
    [SerializeField] private float moveSpeed;
    private Vector3 initScale;
    private bool movingLeft;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        initScale = enemyPos.localScale;   
        moveSpeed = 4;
    }
    private void OnDisable()
    {
        StopMoving();
    }

    private void Update()
    {
        if (movingLeft)
        {
            if(enemyPos.position.x >= leftEdge.position.x) 
            {
                MoveInDir(-1);
            }
            else
            {
                DirChange();
            }
        }
        else if(!movingLeft)
        {
            if(enemyPos.position.x <= rightEdge.position.x )
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
        enemyPos.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        enemyPos.position = new Vector3(enemyPos.position.x + Time.deltaTime * dir * moveSpeed, enemyPos.position.y, enemyPos.position.z);
    }

    private void DirChange()
    {
        anim.SetBool("Moving", false);
        movingLeft = !movingLeft;
    }

    private void StopMoving()
    {
        anim.SetBool("Moving", false);
    }

}
