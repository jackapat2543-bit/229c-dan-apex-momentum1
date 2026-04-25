using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Idle, Chase, Attack }

    [Header("AI Settings")]
    public float moveSpeed    = 4f;
    public float attackRange  = 1.5f;
    public float chaseRange   = 8f;
    public float attackCooldown = 1.2f;
    public float jumpForce    = 20f;
    public float mass         = 1f;

    [Header("Ground Check")]
    [SerializeField] private float groundDistCheck = 1f;
    [SerializeField] private LayerMask groundLayer;

    private AIState state = AIState.Idle;
    private Rigidbody2D rb;
    private CombatSystem combat;
    private Transform player;
    private float attackTimer = 0f;
    private bool isGrounded   = false;
    private bool isKnockedBack = false;

    void Start()
    {
        rb     = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        combat = GetComponent<CombatSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isKnockedBack) return;

        attackTimer -= Time.deltaTime;
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundDistCheck, groundLayer);

        float dist = Vector2.Distance(transform.position, player.position);
        if      (dist <= attackRange) state = AIState.Attack;
        else if (dist <= chaseRange)  state = AIState.Chase;
        else                          state = AIState.Idle;

        switch (state)
        {
            case AIState.Chase:  ChasePlayer();  break;
            case AIState.Attack: AttackPlayer(); break;
            default: rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); break;
        }
    }

    void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        transform.rotation = Quaternion.Euler(0f, dir > 0 ? 0f : 180f, 0f);

        // กระโดดถ้า Player อยู่สูงกว่า
        if (player.position.y > transform.position.y + 1f && isGrounded)
            rb.AddForce(Vector2.up * (mass * jumpForce), ForceMode2D.Impulse);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (attackTimer <= 0f)
        {
            combat?.Attack();
            attackTimer = attackCooldown;
        }
    }

    public void SetKnockedBack(bool val) { isKnockedBack = val; }
}