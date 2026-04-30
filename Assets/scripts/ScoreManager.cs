using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public bool isDead = false;
    [Header("Score")]
    public int score = 0;
    public Text scoreText;

    [Header("Player Health")]
    public int maxHP = 3;
    public int currentHP;
    public Text hpText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentHP = maxHP;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdateUI();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        SceneManager.LoadScene("LoseScene");
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (hpText != null)
            hpText.text = "HP: " + currentHP;
    }
    

}