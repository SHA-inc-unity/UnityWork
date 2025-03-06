using UnityEngine;

public class Grounder : MonoBehaviour
{
    private bool isGrounded;

    public bool IsGrounded { get => isGrounded; }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
