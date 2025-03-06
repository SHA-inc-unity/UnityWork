using UnityEngine;

public class RandPoop : MonoBehaviour
{
    [SerializeField]
    private GameObject poop;

    void FixedUpdate()
    {
        if(Random.Range(0, 10) == 5)
        {
            poop.transform.position = new Vector3(Random.Range(-4f, 4f), poop.transform.position.y, Random.Range(-4f, 4f));
        }
    }
}
