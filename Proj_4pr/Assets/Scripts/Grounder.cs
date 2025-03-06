using System;
using UnityEngine;

public class Grounder : MonoBehaviour
{
    public event Action<GameObject> OnGroundChanged;

    [SerializeField] private string isGrounded = "null"; 
    [SerializeField] private GameObject ground;     

    public string IsGrounded => isGrounded;
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
        if (other.CompareTag("Ground") && isGrounded != null)
        {
            isGrounded = "null";
            Ground = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground") && isGrounded != other.tag)
        {
            isGrounded = other.tag;
            Ground = other.gameObject;
        }
    }
}
