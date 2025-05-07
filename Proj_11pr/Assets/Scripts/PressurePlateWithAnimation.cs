using UnityEngine;
using UnityEngine.Audio;

public class PressurePlateWithAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ParticleSystem targetObject;
    [SerializeField] private bool deactivateOnExit = true;
    [SerializeField] private float moveDistance = 0.1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AudioSource audioSource;

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        initialPosition = transform.localPosition;
        pressedPosition = initialPosition - new Vector3(0, moveDistance, 0);
        targetObject.Stop();
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
            audioSource.Stop();
            audioSource.time = 0.5f;
            audioSource.Play();
            Debug.Log("Объект активирован!");
        }
    }

    private void DeactivateTarget()
    {
        if (targetObject != null)
        {
            targetObject.Stop();
            audioSource.Stop();
            audioSource.time = 0.5f;
            audioSource.Play();
            Debug.Log("Объект деактивирован!");
        }
    }
}