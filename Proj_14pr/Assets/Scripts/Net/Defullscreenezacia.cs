using UnityEngine;

public class Defullscreenezacia : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreen = false; // ��������� fullscreen
        Screen.SetResolution(1280, 720, false); // ������, ������, fullscreen = false
    }

}
