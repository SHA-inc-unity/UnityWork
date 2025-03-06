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
    private Transform standartParent;

    private float timer = 0;
    private Vector3 add = new Vector3(), jump = new Vector3();

    private void Start()
    {
        grounder.OnGroundChanged += OnGroundChanged;
    }

    private void OnDestroy()
    {
        grounder.OnGroundChanged -= OnGroundChanged;
    }

    private void OnGroundChanged(GameObject newGround)
    {
        if (newGround != null)
        {
            transform.parent = newGround.transform;
        }
        else
        {
            transform.parent = standartParent;
        }
    }

    void Update()
    {
        add = new Vector3();

        ClickCheker();

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

        if (grounder.IsGrounded != "null")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (timer == 0)
                {
                    jump = transform.up * 100;
                    timer = timerJumpReload;
                }
            }
        }
    }
}
