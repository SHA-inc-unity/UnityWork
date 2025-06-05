using UnityEngine;
using Unity.Netcode;

public class MovePlayer : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private Transform standartParent;
    [SerializeField] private Player player;
    [SerializeField] private Grounder grounder;
    [SerializeField] private Transform cameraPivot;
    private Transform mainCamera;
    [SerializeField] private Vector3 thirdPersonOffset;
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float minZoom = 0f;
    [SerializeField] private float maxZoom = 3f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipMove, audioClipDirt, audioClipBlue;

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    private bool isFalling = false;
    private float fallStartHeight = 0f;
    private float lastGroundedTime;
    private float currentZoom = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool canJump => Time.time - lastGroundedTime < coyoteTime && grounder.IsGrounded != GroundedType.Dirt;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            mainCamera = Camera.main.transform;
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            if (mainCamera != null && cameraPivot != null)
            {
                currentZoom = maxZoom;
            }

            //grounder.OnGroundChanged += OnGroundChanged;
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnDestroy()
    {
        if (IsOwner)
        {
            //grounder.OnGroundChanged -= OnGroundChanged;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        Move();
        Jump();
        RotateCamera();
        UpdateCameraPosition();
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
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeW)) moveDirection += forward;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeS)) moveDirection -= forward;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeD)) moveDirection += right;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeA)) moveDirection -= right;

        moveDirection.Normalize();
        float calculatedSpeed = speed;

        if (grounder.IsGrounded == GroundedType.Dirt)
        {
            if (audioSource.clip != audioClipDirt)
                audioSource.clip = audioClipDirt;
            calculatedSpeed *= 0.5f;
        }
        else if (grounder.IsGrounded == GroundedType.Jumper)
        {
            if (audioSource.clip != audioClipBlue)
                audioSource.clip = audioClipBlue;
            calculatedSpeed *= 0.75f;
        }
        else if (audioSource.clip != audioClipMove)
            audioSource.clip = audioClipMove;

        if (moveDirection != Vector3.zero)
            audioSource.UnPause();
        else
            audioSource.Pause();

        if (!audioSource.isPlaying && moveDirection != Vector3.zero)
            audioSource.Play();

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

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX * rotationSpeed;
        xRotation -= mouseY * rotationSpeed;

        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        cameraPivot.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void UpdateCameraPosition()
    {
        if (mainCamera == null || cameraPivot == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (currentZoom <= minZoom)
        {
            mainCamera.position = cameraPivot.position;
            mainCamera.rotation = cameraPivot.rotation;
        }
        else
        {
            Vector3 targetPosition = cameraPivot.position - cameraPivot.forward * currentZoom * thirdPersonOffset.magnitude;
            mainCamera.position = Vector3.Lerp(mainCamera.position, targetPosition, Time.deltaTime * cameraSpeed);
            mainCamera.LookAt(cameraPivot.position);
        }
    }

    //private void OnGroundChanged(GameObject newGround)
    //{
    //    if (!IsOwner) return;

    //    if (newGround != null)
    //    {
    //        if (isFalling)
    //        {
    //            float fallDistance = fallStartHeight - transform.position.y;
    //            if (fallDistance > 0.5f)
    //            {
    //                Debug.Log($"Высота падения: {fallDistance} метров");
    //                if (grounder.IsGrounded != GroundedType.Jumper)
    //                {
    //                    Debug.Log($"урон у нас: {(int)fallDistance}");
    //                    player.TakeDamage((int)fallDistance);
    //                }
    //            }
    //            isFalling = false;
    //        }
    //        transform.parent = newGround.transform;
    //    }
    //    else
    //    {
    //        transform.parent = standartParent;
    //    }
    //}
}
