using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerDamage : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public int attack;
    public HealthBar healthBar;


    private Rigidbody rb;
    public float pushBackForce = 15f; // Adjust this to your liking

    // Animator related variables
    private Animator animator;

    public EnemyController EnemyAttackActions;
    public EnemyNoTriggerCollision noTriggerCollision;
	public EnemyTriggerCollision triggerCollision;

    public string attackName;
    public bool dealingDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        attackName = EnemyAttackActions.attackName;
        dealingDamage = EnemyAttackActions.dealingDamage;
        // PlayDamageReaction();
        if(health <= 0)
        {
            animator.SetTrigger("Death");
        }
    }



    // Methods to call in Python
    public int GetHealth()
    {
        return health;
    }

    // Method to set health (if needed)
    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }



    public void TakeDamage(int amount, GameObject damageDealer)
    {
        health -= amount;
        healthBar.SetHealth(health);
        PlayDamageReaction();
        // Push back logic when damaged by an enemy
        if (damageDealer.CompareTag("Enemy"))
        {
            // Calculate pushback direction
            Vector3 pushDirection = (transform.position - damageDealer.transform.position).normalized;

            // Apply the force to the player bot's Rigidbody
            rb.AddForce(pushDirection * pushBackForce, ForceMode.Impulse);
        }
    }

    public void DealDamage(GameObject target, int attack)
    {
        var atm = target.GetComponent<EnemyDamage>();
        if(atm != null && !EnemyAttackActions.isBlocking)
        {
            atm.TakeDamage(attack, gameObject);
        } 

    }

    private void PlayDamageReaction()
    {
        if(attackName == "UpStrong" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageUpStrong");  // Trigger the damage reaction animation
            attackName = "";
        }
        else if(attackName == "UpLight" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageUpLight");  // Trigger the damage reaction animation
            attackName = "";
        }
        else if(attackName == "ForwardLight" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageForwardLight");  // Trigger the damage reaction animation
            attackName = "";
        }
        else if(attackName == "DownLight" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageDownLight");  // Trigger the damage reaction animation
            attackName = "";
        }
        else if(attackName == "ForwardStrong" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageForwardStrong");  // Trigger the damage reaction animation
            attackName = "";
        }
        else if(attackName == "DownStrong" && dealingDamage && triggerCollision.isCollided || noTriggerCollision.isCollided)
        {
            animator.SetTrigger("TakeDamageDownStrong");  // Trigger the damage reaction animation
            attackName = "";
        }
    }

}
