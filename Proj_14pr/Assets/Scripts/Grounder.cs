using System;
using UnityEngine;

public enum GroundedType
{
    Ground,
    Dirt,
    Jumper,
    Null,
}

public class Grounder : MonoBehaviour
{
    public event Action<GameObject> OnGroundChanged;

    [SerializeField] private GroundedType isGrounded = GroundedType.Null;
    [SerializeField] private GameObject ground;

    public GroundedType IsGrounded => isGrounded;
    public GameObject Ground
    {
        get => ground;
        private set
        {
            if (ground != value)
            {
                ground = value;
                OnGroundChanged?.Invoke(ground);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Dirt") || other.CompareTag("Jumper"))
        {
            isGrounded = GroundedType.Null;
            Ground = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
            isGrounded = GroundedType.Ground;
        else if (other.CompareTag("Dirt"))
            isGrounded = GroundedType.Dirt;
        else if (other.CompareTag("Jumper"))
            isGrounded = GroundedType.Jumper;
        else
            return;

        Ground = other.gameObject;
    }
}
