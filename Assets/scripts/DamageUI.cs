using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageUI : MonoBehaviour
{
    public KnockbackReceiver Player;
    public TMP_Text DamageText;

    void Update()
    {
        DamageText.text = Player.damagePercent.ToString("F0") + "%";
    }
}