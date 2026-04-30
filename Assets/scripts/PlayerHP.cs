using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    public bool isDead = false;
    public string loseSceneName = "LoseScene"; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.GetComponent<EnemyAI>() != null)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.GetComponent<EnemyAI>() != null)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died");

        

        Invoke(nameof(LoadLoseScene), 1f);
    }

    void LoadLoseScene()
    {
        SceneManager.LoadScene(loseSceneName);
    }
}