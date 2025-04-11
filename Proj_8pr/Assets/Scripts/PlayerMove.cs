using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private void Update()
    {
        bool isNowMove = false;

        if (Input.GetKey(KeyCode.W))
        {
            isNowMove = true;
            animator.SetBool("forward", true);
        }
        else
            animator.SetBool("forward", false);

        if (Input.GetKey(KeyCode.S))
        {
            isNowMove = true;
            animator.SetBool("backward", true);
        }
        else
            animator.SetBool("backward", false);

        if (Input.GetKey(KeyCode.D))
        {
            isNowMove = true;
            animator.SetBool("right", true);
        }
        else
            animator.SetBool("right", false);

        if (Input.GetKey(KeyCode.A))
        {
            isNowMove = true;
            animator.SetBool("left", true);
        }
        else
            animator.SetBool("left", false);

        animator.SetBool("isMove", isNowMove);
    }
}
