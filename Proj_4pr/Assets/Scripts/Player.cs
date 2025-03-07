
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private MovePlayer movePlayer;
    [SerializeField]
    private HealthController healthController;

    public void TakeDamage(int damage)
    {
        healthController.TakeDamage(damage);
    }
}
