using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [Header("Hit Settings")]
    public float hitCooldown = 1f;

    private float lastHitTime = -999f;
    private bool isDead = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead) return;
        if (col.gameObject.GetComponent<EnemyAI>() != null)
            TryTakeDamage();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead) return;
        if (col.GetComponent<EnemyAI>() != null)
            TryTakeDamage();
    }

    void TryTakeDamage()
    {
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.TakeDamage(1);
    }

    public void SetDead() { isDead = true; }
}