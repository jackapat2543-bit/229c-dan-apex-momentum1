using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Setting")]
    public float damagePercent = 0f; // เปอร์เซ็นความเสียหาย
    public float mass = 1f;          // มวล (ใช้ในสูตร F = m × a)

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction, float basePower)
    {
        // เพิ่ม damage ทุกครั้งที่โดนตี
        damagePercent += 10f;

        // ══ Physics D: Newton’s 2nd Law ══
        // F = m × a
        float acceleration = basePower * (1f + damagePercent / 100f);
        float force = mass * acceleration;

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }
}