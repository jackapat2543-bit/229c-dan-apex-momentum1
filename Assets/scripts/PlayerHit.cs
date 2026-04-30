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
        
        ScoreManager.Instance.TakeDamage(1);

        
        Vector2 dir = (transform.position - enemy.position).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
    }
void Update()
{
    if (ScoreManager.Instance.isDead)
    {
        FindObjectOfType<MenuManager>().load_lose_scene();
    }
}

}

