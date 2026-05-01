using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Setting")]
    public float damagePercent = 0f;
    public float mass = 1f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = false; 
    }

    public void ApplyKnockback(Vector2 direction, float basePower)
    {
        damagePercent += 10f;

        // ══ Physics D: Newton's 2nd Law ══
        // F = m × a  (ของเดิม — ถูกต้องแล้ว)
        float acceleration = basePower * (1f + damagePercent / 100f);
        float force = mass * acceleration;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        // ══ Physics F: Rotational Motion ══  ← เพิ่มส่วนนี้
        // Vector3.Cross หาแกนการหมุนจากทิศ knockback
        Vector3 dir3D = new Vector3(direction.x, direction.y, 0f);
        Vector3 angular = Vector3.Cross(dir3D, Vector3.forward);
        rb.angularVelocity = angular.z * force * 30f;
        // ตีจากซ้าย → หมุนทวนเข็ม / ตีจากขวา → หมุนตามเข็ม
    }

    public void ResetDamage()
    {
        damagePercent = 0f;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;
    }
 
}