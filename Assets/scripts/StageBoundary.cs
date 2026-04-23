using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBoundary : MonoBehaviour
{
    public int playerScore = 0;
    public int enemyScore = 0;
    public int winScore = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyScore++;
            Debug.Log("Enemy Score: " + enemyScore);
            CheckWin();
        }
        else if (other.CompareTag("Enemy"))
        {
            playerScore++;
            Debug.Log("Player Score: " + playerScore);
            CheckWin();
        }
    }

    void CheckWin()
    {
        if (playerScore >= winScore)
        {
            Debug.Log("PLAYER WIN");
            // ใส่ scene ชนะทีหลัง
        }
        else if (enemyScore >= winScore)
        {
            Debug.Log("ENEMY WIN");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}