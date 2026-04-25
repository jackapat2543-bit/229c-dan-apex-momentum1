using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage  = 10f;
    public float basePunchForce = 8f;
    public float attackDuration = 0.2f;
    public GameObject hitEffect;

    [Header("References")]
    public Collider2D hitbox; // ลาก HitboxObject Collider มาใส่

    private bool isAttacking = false;

    void Start() { if (hitbox) hitbox.enabled = false; }

    public void Attack()
    {
        if (!isAttacking) StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        if (hitbox) hitbox.enabled = true;

        float timer = 0f;
        while (timer < attackDuration)
        {
            // ขยับ hitbox ตามทิศที่หันหน้า
            float dir = transform.localScale.x > 0 ? 1f : -1f;
            if (hitbox) hitbox.transform.localPosition = new Vector2(dir * 1f, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        if (hitbox) hitbox.enabled = false;
        yield return new WaitForSeconds(0.2f); // cooldown
        isAttacking = false;
    }

    // ══ Physics A: Trigger2D ══
    void OnTriggerEnter2D(Collider2D other)
    {
        KnockbackReceiver kb = other.GetComponent<KnockbackReceiver>();
        if (kb == null || other.gameObject == gameObject) return;

        Vector2 hitDir = new Vector2(transform.localScale.x > 0 ? 1f : -1f, 0.4f).normalized;
        kb.ApplyKnockback(hitDir, basePunchForce);

        if (hitEffect) Instantiate(hitEffect, other.transform.position, Quaternion.identity);
    }
}