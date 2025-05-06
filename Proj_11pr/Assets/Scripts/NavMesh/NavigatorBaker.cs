using Unity.AI.Navigation;
using UnityEngine;

public class NavigatorBaker : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface.BuildNavMesh();
    }
}
