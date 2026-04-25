using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBoundary : MonoBehaviour
{
    public int playerScore = 0;
    public int enemyScore  = 0;
    public int winScore    = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyScore++;
            other.GetComponent<KnockbackReceiver>()?.ResetDamage(); // reset damage%
            other.transform.position = new Vector2(0f, 2f);           // respawn
            CheckWin();
        }
        else if (other.CompareTag("Enemy"))
        {
            playerScore++;
            other.GetComponent<KnockbackReceiver>()?.ResetDamage();
            other.transform.position = new Vector2(3f, 2f);
            CheckWin();
        }

        // อัพเดต UI score (ถ้ามี GameManager)
        GameManager.instance?.UpdateScoreUI(playerScore, enemyScore);
    }

    void CheckWin()
    {
        if (playerScore >= winScore)
        {
            PlayerPrefs.SetInt("PlayerWin", 1);
            SceneManager.LoadScene("Result"); // ← เปลี่ยนจาก Debug.Log
        }
        else if (enemyScore >= winScore)
        {
            PlayerPrefs.SetInt("PlayerWin", 0);
            SceneManager.LoadScene("Result"); // ← เปลี่ยนจาก Debug.Log
        }
        // ถ้ายังไม่ถึง winScore → respawn อัตโนมัติจากด้านบนแล้ว
    }
}