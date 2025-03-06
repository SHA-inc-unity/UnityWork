using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Transform obj;
    [SerializeField]
    private Vector3 pos, posFPV;
    [SerializeField]
    private Quaternion rotationStandart, rotationFPV;
    [SerializeField]
    private float speed = 5;

    private bool isFPV;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            isFPV = !isFPV;

        if (isFPV) 
        {
            transform.position = Vector3.MoveTowards(transform.position, obj.position + posFPV, speed * Time.deltaTime);
            transform.rotation = rotationFPV;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, obj.position + pos, speed * Time.deltaTime);
            transform.rotation = rotationStandart;
        }

    }
}
