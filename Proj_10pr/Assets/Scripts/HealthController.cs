using System.Linq;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private HeartController[] heartControllers;
    [SerializeField]
    private float maxHealth = 100f, maxHeartHealth = 10f;

    private float currentHealth;

    private void Awake()
    {
        foreach (var heart in heartControllers)
        {
            heart.ForceAwake(maxHeartHealth);
        }

        currentHealth = maxHealth;
        CheckHealth();
    }

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        CheckHealth();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        CheckHealth();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        CheckHealth();
    }

    public void CheckHealth()
    {
        int fullHearts = Mathf.FloorToInt(currentHealth / maxHeartHealth);
        float remainder = currentHealth % maxHeartHealth;

        for (int i = 0; i < heartControllers.Length; i++)
        {
            if (i < fullHearts)
            {
                heartControllers[i].Health = maxHeartHealth;
            }
            else if (i == fullHearts)
            {
                heartControllers[i].Health = remainder;
            }
            else
            {
                heartControllers[i].Health = 0f;
            }
        }
    }
}
