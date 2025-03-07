using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    [SerializeField]
    private RectTransform heartFill;
    [SerializeField]
    private float health = 3f;
    [SerializeField]
    private float maxHp = 10f; // Максимальное HP одного сердца

    private float maxHeight;

    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0f, maxHp);
            UpdateHeartUI();
        }
    }

    public void ForceAwake(float hp)
    {
        maxHp = hp;
        maxHeight = heartFill.sizeDelta.y;
        UpdateHeartUI();
    }

    private void UpdateHeartUI()
    {
        float fillPercent = health / maxHp;
        heartFill.sizeDelta = new Vector2(heartFill.sizeDelta.x, maxHeight * fillPercent);
    }
}
