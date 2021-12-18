using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatTestDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject hitParticles;
    
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        
    }

    public void Damage(float amount)
    {
        Debug.Log(amount + " Damage Taken");

        Instantiate(hitParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        _anim.SetTrigger("damage");
    }
}
