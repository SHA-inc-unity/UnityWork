using UnityEngine;

public class ButtonsPresets
{
    private static ButtonsPresets instance;
    public static ButtonsPresets Instance
    {
        get
        {
            if (instance == null) instance = new ButtonsPresets();
            return instance;
        }
    }

    private KeyCode keyCodeA = KeyCode.A;
    private KeyCode keyCodeW = KeyCode.W;
    private KeyCode keyCodeS = KeyCode.S;
    private KeyCode keyCodeD = KeyCode.D;

    public KeyCode KeyCodeA => keyCodeA;
    public KeyCode KeyCodeW => keyCodeW;
    public KeyCode KeyCodeS => keyCodeS;
    public KeyCode KeyCodeD => keyCodeD;

    public void SetKeyA(KeyCode newKey) => keyCodeA = newKey;
    public void SetKeyW(KeyCode newKey) => keyCodeW = newKey;
    public void SetKeyS(KeyCode newKey) => keyCodeS = newKey;
    public void SetKeyD(KeyCode newKey) => keyCodeD = newKey;
}
