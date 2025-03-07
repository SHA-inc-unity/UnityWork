using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpPower = 7f;
    [SerializeField]
    private float gravityMultiplier = 2f;
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    private Transform standartParent;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Grounder grounder;

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    private bool isFalling = false;
    private float fallStartHeight = 0f;
    private float lastGroundedTime;
    private bool canJump => Time.time - lastGroundedTime < coyoteTime && grounder.IsGrounded != GroundedType.Dirt;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
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
            if (isFalling)
            {
                float fallDistance = fallStartHeight - transform.position.y;
                if (fallDistance > 0.5f)
                {
                    Debug.Log($"Высота падения: {fallDistance} метров");
                    if (grounder.IsGrounded != GroundedType.Jumper)
                    {
                        player.TakeDamage((int)fallDistance);
                    }
                }
                isFalling = false;
            }
            transform.parent = newGround.transform;
        }
        else
        {
            transform.parent = standartParent;
        }
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= forward;
        if (Input.GetKey(KeyCode.D)) moveDirection += right;
        if (Input.GetKey(KeyCode.A)) moveDirection -= right;

        moveDirection.Normalize();
        float calculatedSpeed = speed;
        if (grounder.IsGrounded == GroundedType.Dirt)
            calculatedSpeed *= 0.5f;
        else if (grounder.IsGrounded == GroundedType.Jumper)
            calculatedSpeed *= 0.75f;

        rb.linearVelocity = new Vector3(moveDirection.x * calculatedSpeed, rb.linearVelocity.y, moveDirection.z * calculatedSpeed);
    }

    private void Jump()
    {
        if (grounder.IsGrounded != GroundedType.Null)
        {
            lastGroundedTime = Time.time;
            isFalling = false;
        }
        else if (!isFalling)
        {
            isFalling = true;
            fallStartHeight = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            float jumpMultiplier = (grounder.IsGrounded == GroundedType.Jumper) ? 2.5f : 1f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower * jumpMultiplier, rb.linearVelocity.z);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
    }
}
