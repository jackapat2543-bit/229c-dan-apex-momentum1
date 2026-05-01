using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    public int score = 0;
    public TMP_Text scoreText;

    [Header("Player Health")]
    public int maxHP = 3;
    public int currentHP;
    public TMP_Text hpText;

    [Header("Scene")]
    public string loseSceneName = "LoseScene";

    private bool isDead = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        currentHP = maxHP;
        UpdateUI();
    }

    public void AddScore(int amount)
{
    if (isDead) return;

    score += amount;
    UpdateUI();

    if (score >= winScore)
    {
        Win();
    }
}

    
    public void TakeDamage(int dmg = 1)
    {
        if (isDead) return;
        currentHP -= dmg;
        UpdateUI();

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        Invoke(nameof(LoadLose), 1f);
    }

    void LoadLose() => SceneManager.LoadScene(3); 

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (hpText)    hpText.text    = "HP: " + currentHP;
    }
[Header("Win Condition")]
public int winScore = 10;
public string winSceneName = "WinScene";

void Win()
{
    isDead = true; 
    Invoke(nameof(LoadWin), 1f);
}

void LoadWin()
{
    SceneManager.LoadScene(4);
}


}