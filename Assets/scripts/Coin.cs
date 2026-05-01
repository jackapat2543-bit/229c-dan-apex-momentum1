using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    public int scoreValue = 10;      // คะแนนที่ได้
    public GameObject collectEffect; // Particle ตอนเก็บ (ถ้ามี)

    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจว่า Player เข้ามาชน
        if (!other.CompareTag("Player")) return;

        // เพิ่ม Score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        // Spawn Effect
        if (collectEffect)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // ลบเหรียญออก
        Destroy(gameObject);
    }
}