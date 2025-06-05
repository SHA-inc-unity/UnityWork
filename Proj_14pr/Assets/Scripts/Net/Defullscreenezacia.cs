using UnityEngine;

public class Defullscreenezacia : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreen = false; // отключить fullscreen
        Screen.SetResolution(1280, 720, false); // ширина, высота, fullscreen = false
    }

}
