using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{

    private Animator animator;
    private float attackDmg;
    private float attackDelay;
    Movement 

    void Start()
    {
        animator = GetComponent<Animator>();
        attackDmg = 14f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

        }
    }
}
