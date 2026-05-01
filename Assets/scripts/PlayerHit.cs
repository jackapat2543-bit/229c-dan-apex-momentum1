using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public float knockbackForce = 8f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyAI>() != null)
        {
            TakeHit(collision.transform);
        }
    }

    void TakeHit(Transform enemy)
    {
        // ลด HP
        ScoreManager.Instance.TakeDamage(1);

        // คำนวณทิศกระเด็น
        Vector2 dir = (transform.position - enemy.position).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
    }
}