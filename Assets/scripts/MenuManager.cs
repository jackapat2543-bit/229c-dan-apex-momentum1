using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Result Screen (ใส่ถ้าอยู่ใน Result Scene)")]
    public TMP_Text resultText;

    void Start()
    {
        if (resultText)
        {
            int win = PlayerPrefs.GetInt("PlayerWin", 0);
            resultText.text = win == 1 ? "🏆 YOU WIN!" : "💀 YOU LOSE...";
        }
    }

    // ผูกกับ Button → OnClick() ใน Inspector
    public void PlayGame()  => SceneManager.LoadScene("Battle");
    public void GoToMenu() => SceneManager.LoadScene("MainMenu");
    public void GoCredit() => SceneManager.LoadScene("Credit");
}