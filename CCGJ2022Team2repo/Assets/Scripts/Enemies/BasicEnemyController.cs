using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State{ Moving, Knockback, Dead }
    private State currentState;
    
    [SerializeField] private float
        groundCheckDistance, wallCheckDistance,
        movementSpeed, maxHealth, knockBackDuration;
    [SerializeField] private Transform groundCheck, wallCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Vector2 knockBackSpeed;

    // Hope this works
    [SerializeField] private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    private float currentHealth, knockBackStartTime;
    private int facingDirection, damageDirection;
    private Vector2 movement;
    private bool groundDetected, wallDetected;
    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    private void Start() {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }
    //--WALKING STATE------------------------------------------------------------------------------------------------------------------------
    private void EnterMovingState()
    {

    }
    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
            flip();
        else {
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }
    private void ExitMovingState()
    {

    }

    //--KNOCKBACK STATE------------------------------------------------------------------------------------------------------------------------
    private void EnterKnockbackState()
    {
        knockBackStartTime = Time.time;
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("KnockBack", true);
    }
    private void UpdateKnockbackState()
    {
        if (Time.time >= knockBackStartTime + knockBackDuration)
            SwitchState(State.Moving);
    }
    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("KnockBack", false);
    }

    //--DEAD STATE------------------------------------------------------------------------------------------------------------------------
    private void EnterDeadState()
    {
        Instantiate(deathChunkParticle, alive.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, alive.transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {

    }

    //--OTHER FUNCTIONS-----------------------------------------------------------------------------------------------------------------------
    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        if (attackDetails[1] > alive.transform.position.x)
            damageDirection = -1;
        else
            damageDirection = 1;

        //hit particle

        if (currentHealth > 0.0f)
            SwitchState(State.Knockback);
        else if (currentHealth <= 0.0f)
            SwitchState(State.Dead);
    }
    private void flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }
        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
