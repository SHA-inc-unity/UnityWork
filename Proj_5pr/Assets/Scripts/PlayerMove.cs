using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runMultiple = 2f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float minZoomMultiplier = 0f;
    [SerializeField] private float maxZoomMultiplier = 3f;
    [SerializeField] private float zoomSpeed = 25f;

    private float xRotation = 0f;
    private float currentZoom = 5f;
    private Vector3 initialCameraOffset;
    [Header ("Fight")]

    [SerializeField] private int maxComboCount = 4;
    [SerializeField] private float comboResetTime = 1f;

    private int currentComboIndex = 0;
    private float comboTimer = 0f;
    private bool comboNext = false;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCamera != null && cameraPivot != null)
        {
            initialCameraOffset = mainCamera.localPosition;
            currentZoom = 1f;
        }
    }


    private void Update()
    {
        RotateCamera();
        MovePlayer();
        ZoomCamera();

        PunchEnemy();
    }


    private void PunchEnemy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            comboNext = true;
            if(currentComboIndex == 0)
            {
                NextStepCombat();
            }
        }

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
            {
                if (comboNext)
                {
                    NextStepCombat();
                }
                else
                {
                    currentComboIndex = 0;
                    animator.SetFloat("Combo", 0);
                }
            }
        }
    }

    private void NextStepCombat()
    {
        currentComboIndex++;
        if (currentComboIndex > maxComboCount)
        {
            currentComboIndex = 1;
        }
        animator.SetFloat("Combo", currentComboIndex);
        Debug.Log(animator.GetFloat("Combo"));

        comboTimer = comboResetTime;
        comboNext = false;
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 40f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void ZoomCamera()
    {
        if (mainCamera == null || cameraPivot == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, minZoomMultiplier, maxZoomMultiplier);

        mainCamera.localPosition = initialCameraOffset * currentZoom;
    }

    private void MovePlayer()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
        if (Input.GetKey(KeyCode.D)) direction += Vector2.right;
        if (Input.GetKey(KeyCode.A)) direction += Vector2.left;

        bool isRun = Input.GetKey(KeyCode.LeftShift);
        bool isNowMove = direction != Vector2.zero;

        if (isNowMove)
            HandleMovement(direction, isRun);

        animator.SetBool("isMove", isNowMove);
    }

    private void HandleMovement(Vector2 direction, bool isRunning)
    {
        direction.Normalize();

        Vector3 moveDirection = transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        float currentSpeed = isRunning ? speed * runMultiple : speed;

        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        float blend = angle / 360f;

        animator.SetFloat("Blend", blend);
    }
}
