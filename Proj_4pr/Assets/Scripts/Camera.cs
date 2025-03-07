using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target; // Игрок
    [SerializeField]
    private Vector3 pos, posFPV;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float rotationSpeed = 100f; // Скорость вращения камеры
    [SerializeField]
    private float smoothRotation = 5f; // Плавность вращения

    private bool isFPV;
    private float currentRotationY;
    private Quaternion lastRotation; // Последний угол камеры перед входом в FPV

    private void Start()
    {
        currentRotationY = target.eulerAngles.y;
        lastRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isFPV = !isFPV;
            if (isFPV)
            {
                lastRotation = transform.rotation; // Запоминаем текущий угол камеры перед входом в FPV
            }
        }

        if (isFPV)
        {
            // Камера в FPV-режиме остаётся с тем же углом, который был перед входом
            transform.position = Vector3.MoveTowards(transform.position, target.position + posFPV, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, lastRotation, Time.deltaTime * smoothRotation);
        }
        else
        {
            HandleRotation();
            Vector3 desiredPosition = target.position + Quaternion.Euler(0, currentRotationY, 0) * pos;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, speed * Time.deltaTime);
            transform.LookAt(target.position);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            currentRotationY += rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentRotationY -= rotationSpeed * Time.deltaTime;
        }
    }
}
