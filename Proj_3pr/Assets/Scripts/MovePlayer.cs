using Unity.VisualScripting;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Grounder grounder;
    [SerializeField]
    private float timerJumpReload = 30, timerJumpFly = 10;
    [SerializeField]
    Animator animator;

    private float timer = 0;
    private Vector3 add = new Vector3(), jump = new Vector3();

    void Update()
    {
        add = new Vector3();

        ClickCheker();

        if (add != new Vector3() || timer != 0)
            animator.SetBool("isMove", true);
        else
            animator.SetBool("isMove", false);

        if (timer > timerJumpReload - timerJumpFly)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + add + jump, speed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, transform.position + add, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (timer > 0)
            timer--;
    }

    private void ClickCheker()
    {
        if (Input.GetKey(KeyCode.W))
            add += transform.forward;
        if (Input.GetKey(KeyCode.S))
            add -= transform.forward;

        if (Input.GetKey(KeyCode.D))
            add += transform.right;
        if (Input.GetKey(KeyCode.A))
            add -= transform.right;

        if (grounder.IsGrounded && Input.GetKey(KeyCode.Space))
            if (timer == 0)
            {
                jump = transform.up * 100;
                timer = timerJumpReload;
            }
    }
}
