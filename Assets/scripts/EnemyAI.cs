using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Idle, Chase, Attack, Retreat, Stunned }

    [Header("AI Settings")]
    public float moveSpeed      = 4f;
    public float attackRange    = 1.5f;
    public float chaseRange     = 8f;
    public float retreatRange   = 2.5f;   // ระยะที่เริ่มถอยถ้า damage% สูง
    public float retreatDamageThreshold = 60f; // % ที่เริ่มถอย
    public float attackCooldown = 1.2f;
    public float jumpForce      = 20f;
    public float mass           = 1f;

    [Header("Wall / Stuck Detection")]
    public float wallCheckDist  = 0.4f;   // ระยะตรวจกำแพงข้างหน้า
    public float stuckTimeout   = 0.6f;   // วินาทีที่ถือว่า "ติด"
    public LayerMask wallLayer;

    [Header("Ground Check")]
    public float groundDist     = 1.1f;
    public LayerMask groundLayer;

    [Header("Stun Settings")]
    public float stunVelocityThreshold = 12f; // velocity ที่ต้องมีก่อนชนกำแพงถึงจะ stun
    public float stunDuration   = 1.2f;

    // ── Private ──────────────────────────────
    private AIState state       = AIState.Idle;
    private Rigidbody2D rb;
    private CombatSystem combat;
    private KnockbackReceiver kb;
    private Transform player;

    private float attackTimer   = 0f;
    private bool  isGrounded    = false;
    private bool  isKnockedBack = false;

    // Stuck detection
    private Vector2 lastPos;
    private float   stuckTimer  = 0f;

    // Stun
    private float   stunTimer   = 0f;
    private bool    wasKnockingBack = false; // ใช้ตรวจว่าเพิ่งกระเด็นอยู่

    // ─────────────────────────────────────────

    void Start()
    {
        rb     = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        kb     = GetComponent<KnockbackReceiver>();
        combat = GetComponent<CombatSystem>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;

        lastPos = transform.position;
    }

    void Update()
    {
        if (isKnockedBack)
{
    return;
} 
        if (player == null) return;

        attackTimer -= Time.deltaTime;
        isGrounded   = Physics2D.Raycast(transform.position, Vector2.down, groundDist, groundLayer);

        // ── ตรวจ Stun หลังชนกำแพง ──
        HandleWallStun();

        // ── ถ้ากำลัง Stun อยู่ ──
        if (state == AIState.Stunned)
        {
            stunTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (stunTimer <= 0f) state = AIState.Idle;
            return;
        }

        // ── ถ้ากำลังกระเด็นอยู่ ──
        if (isKnockedBack) { wasKnockingBack = true; return; }

        // ── เลือก State ──
        float dist   = Vector2.Distance(transform.position, player.position);
        float dmgPct = kb ? kb.damagePercent : 0f;

        if (dmgPct >= retreatDamageThreshold && dist < retreatRange)
            state = AIState.Retreat;
        else if (dist <= attackRange)
            state = AIState.Attack;
        else if (dist <= chaseRange)
            state = AIState.Chase;
        else
            state = AIState.Idle;

        // ── Execute State ──
        switch (state)
        {
            case AIState.Chase:   ChasePlayer();   break;
            case AIState.Attack:  AttackPlayer();  break;
            case AIState.Retreat: RetreatFromPlayer(); break;
            default:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }

        // ── Stuck Detection ──
        CheckStuck();
    }

    // ─── Chase ───────────────────────────────
    void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;

        // ตรวจกำแพงข้างหน้า
        bool wallAhead = Physics2D.Raycast(
            transform.position,
            new Vector2(dir, 0),
            wallCheckDist,
            wallLayer
        );

        if (wallAhead && isGrounded)
        {
            // กระโดดข้ามกำแพง
            rb.AddForce(new Vector2(dir * mass * moveSpeed * 2f, mass * jumpForce), ForceMode2D.Impulse);
        }
        else
        {
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

            // กระโดดถ้า Player อยู่สูงกว่า
            if (player.position.y > transform.position.y + 1.2f && isGrounded)
                rb.AddForce(Vector2.up * (mass * jumpForce), ForceMode2D.Impulse);
        }

        // หันหน้า
        transform.rotation = Quaternion.Euler(0f, dir > 0 ? 0f : 180f, 0f);
    }

    // ─── Attack ──────────────────────────────
    void AttackPlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // หันหน้าหา Player ก่อนตี
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        transform.rotation = Quaternion.Euler(0f, dir > 0 ? 0f : 180f, 0f);

        if (attackTimer <= 0f)
        {
            combat?.Attack();
            attackTimer = attackCooldown;
        }
    }

    // ─── Retreat ─────────────────────────────
    void RetreatFromPlayer()
    {
        // วิ่งหนีทิศตรงข้ามกับ Player
        float dir = player.position.x > transform.position.x ? -1f : 1f;

        bool wallAhead = Physics2D.Raycast(
            transform.position,
            new Vector2(dir, 0),
            wallCheckDist,
            wallLayer
        );

        if (wallAhead && isGrounded)
        {
            // กระโดดข้ามกำแพงตอนหนี
            rb.AddForce(new Vector2(dir * mass * moveSpeed, mass * jumpForce * 0.8f), ForceMode2D.Impulse);
        }
        else
        {
            rb.linearVelocity = new Vector2(dir * moveSpeed * 1.2f, rb.linearVelocity.y);
        }

        transform.rotation = Quaternion.Euler(0f, dir > 0 ? 0f : 180f, 0f);

        // ถ้าหนีไกลพอแล้ว → กลับไป Chase
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist >= retreatRange * 1.5f) state = AIState.Chase;
    }

    // ─── Stun หลังชนกำแพง ────────────────────
    void HandleWallStun()
    {
        // ตรวจว่าเพิ่งกระเด็นอยู่ แล้วหยุดกระทันหัน = ชนกำแพง
        if (wasKnockingBack && !isKnockedBack)
        {
            wasKnockingBack = false;
            float speed = rb.linearVelocity.magnitude;

            if (speed < stunVelocityThreshold * 0.3f)
            {
                // velocity ลดลงเร็วมาก → ชนกำแพงหรือพื้น → Stun!
                state     = AIState.Stunned;
                stunTimer = stunDuration;
                rb.linearVelocity  = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.rotation        = 0f;
            }
        }

        // อัพเดต wasKnockingBack
        if (isKnockedBack) wasKnockingBack = true;
    }

    // ─── Stuck Detection ─────────────────────
    void CheckStuck()
    {
        float moved = Vector2.Distance(transform.position, lastPos);

        if (moved < 0.02f && (state == AIState.Chase || state == AIState.Retreat))
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTimeout && isGrounded)
            {
                // ติดแล้ว → กระโดดหลุด
                rb.AddForce(Vector2.up * (mass * jumpForce * 1.1f), ForceMode2D.Impulse);
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPos = transform.position;
    }

    // ─── Public ──────────────────────────────
    
    public void SetKnockedBack(bool val)
{
    isKnockedBack = val;
}
}