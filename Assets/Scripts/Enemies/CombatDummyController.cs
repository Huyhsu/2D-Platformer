using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatDummyController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth, knockBackSpeedX, knockBackSpeedY, knockBackDuration, knockBackDeathSpeedX, knockBackDeathSpeedY, deathTorque;
    [SerializeField]
    private bool applyKnockBack;
    [SerializeField]
    private GameObject hitParticle;
    
    private float currentHealth, knockBackStart;

    private int playerFacingDirection;

    private bool playerOnLeft, knockBack;

    private PlayerController _playerController;
    private GameObject _aliveGameObject, _brokenTopGameObject, _brokenBottomGameObject;
    private Rigidbody2D _rbAlive, _rbBrokenTop, _rbBrokenBottom;
    private Animator _aliveAnimator;

    private void Start()
    {
        currentHealth = maxHealth;

        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _aliveGameObject = transform.Find("Alive").gameObject;
        _brokenTopGameObject = transform.Find("Broken Top").gameObject;
        _brokenBottomGameObject = transform.Find("Broken Bottom").gameObject;

        _aliveAnimator = _aliveGameObject.GetComponent<Animator>();
        _rbAlive = _aliveGameObject.GetComponent<Rigidbody2D>();
        _rbBrokenTop = _brokenTopGameObject.GetComponent<Rigidbody2D>();
        _rbBrokenBottom = _brokenBottomGameObject.GetComponent<Rigidbody2D>();
        
        _aliveGameObject.SetActive(true);
        _brokenTopGameObject.SetActive(false);
        _brokenBottomGameObject.SetActive(false);
    }

    private void Update()
    {
        CheckKnockBack();
    }

    private void Damage(AttackDetails attackDetails)
    {
        currentHealth -= attackDetails.damageAmount;
        
        // playerFacingDirection = _playerController.GetFacingDirection();
        if (attackDetails.position.x < _aliveGameObject.transform.position.x)
        {
            playerFacingDirection = 1;
        }
        else
        {
            playerFacingDirection = -1;
        }

        Instantiate(hitParticle, _aliveGameObject.transform.position,
            Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        
        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }

        _aliveAnimator.SetBool("playerOnLeft", playerOnLeft);
        _aliveAnimator.SetTrigger("damage");

        if (applyKnockBack && currentHealth > 0.0f)
        {
            //KnockBack
            KnockBack();
        }

        if (currentHealth <= 0.0f)
        {
            //Die
            Die();
        }
    }

    private void KnockBack()
    {
        knockBack = true;
        knockBackStart = Time.time;
        _rbAlive.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
    }

    private void CheckKnockBack()
    {
        if (Time.time >= knockBackStart + knockBackDuration && knockBack)
        {
            knockBack = false;
            _rbAlive.velocity = new Vector2(0.0f, _rbAlive.velocity.y);
        }
    }

    private void Die()
    {
        _aliveGameObject.SetActive(false);
        _brokenTopGameObject.SetActive(true);
        _brokenBottomGameObject.SetActive(true);

        _brokenTopGameObject.transform.position = _aliveGameObject.transform.position;
        _brokenBottomGameObject.transform.position = _aliveGameObject.transform.position;

        _rbBrokenBottom.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
        _rbBrokenTop.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
        _rbBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
    }
}
