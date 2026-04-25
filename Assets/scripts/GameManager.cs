using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI — ลาก Text มาใส่")]
    public TMP_Text playerScoreText;
    public TMP_Text enemyScoreText;
    public TMP_Text roundText;

    public bool isGameActive { get; private set; }

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        isGameActive = true;
    }

    public void UpdateScoreUI(int pScore, int eScore)
    {
        if (playerScoreText) playerScoreText.text = $"P: {pScore}";
        if (enemyScoreText)  enemyScoreText.text  = $"E: {eScore}";
    }

    public void RestartBattle() => SceneManager.LoadScene("Battle");
    public void GoToMenu()      => SceneManager.LoadScene("MainMenu");
}