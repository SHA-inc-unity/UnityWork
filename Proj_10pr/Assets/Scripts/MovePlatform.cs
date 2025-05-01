using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField]
    public List<Transform> positions;
    [SerializeField]
    private Transform platform;
    [SerializeField]
    public float speed = 1f;

    private bool isForward = false;
    private int nowPosDot = 0;

    public void Initialize(List<Transform> positions)
    {
        this.positions = new List<Transform>();
        foreach (var position in positions)
        {
            GameObject dot = new GameObject("MovePoint");
            dot.transform.position = position.position;
            this.positions.Add(dot.transform);
        }
    }

    private void Update()
    {
        if (positions.Count == 0) return;

        platform.transform.position = Vector3.MoveTowards(platform.transform.position, positions[nowPosDot].position, speed * Time.deltaTime);

        if(platform.transform.position == positions[nowPosDot].position)
        {
            if (isForward)
            {
                if(nowPosDot + 1 == positions.Count)
                    isForward = !isForward;
                else
                    nowPosDot++;
            }
            else
            {
                if (nowPosDot == 0)
                    isForward = !isForward;
                else
                    nowPosDot--;
            }
        }
    }
}
