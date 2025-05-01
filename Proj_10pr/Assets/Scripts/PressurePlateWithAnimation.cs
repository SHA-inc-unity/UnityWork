using UnityEngine;

public class PressurePlateWithAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ParticleSystem targetObject;
    [SerializeField] private bool deactivateOnExit = true;
    [SerializeField] private float moveDistance = 0.1f;
    [SerializeField] private float moveSpeed = 5f;

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        initialPosition = transform.localPosition;
        pressedPosition = initialPosition - new Vector3(0, moveDistance, 0); 
        DeactivateTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = true;
            ActivateTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = false;
            if (deactivateOnExit)
                DeactivateTarget();
        }
    }

    private void Update()
    {
        if (isPressed)
            transform.localPosition = Vector3.Lerp(transform.localPosition, pressedPosition, Time.deltaTime * moveSpeed);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * moveSpeed);
    }

    private void ActivateTarget()
    {
        if (targetObject != null)
        {
            targetObject.Play();
            Debug.Log("Объект активирован!");
        }
    }

    private void DeactivateTarget()
    {
        if (targetObject != null)
        {
            targetObject.Stop();
            Debug.Log("Объект деактивирован!");
        }
    }
}