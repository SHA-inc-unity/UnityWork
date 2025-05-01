using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SaveLoaderLevel saveLoader;

    public void Save()
    {
        saveLoader.SaveLevel();
    }
}
