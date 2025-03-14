using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> panels;
    [SerializeField] 
    private TMP_Dropdown dropdownA, dropdownW, dropdownS, dropdownD;

    private void Start()
    {
        ChangePanel(0);
        InitializeDropdowns();
    }

    public void ChangePanel(int num)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(num == i);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }


    private void InitializeDropdowns()
    {
        List<string> keyOptions = new List<string>();
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            keyOptions.Add(key.ToString());
        }

        SetupDropdown(dropdownA, ButtonsPresets.Instance.KeyCodeA, keyOptions);
        SetupDropdown(dropdownW, ButtonsPresets.Instance.KeyCodeW, keyOptions);
        SetupDropdown(dropdownS, ButtonsPresets.Instance.KeyCodeS, keyOptions);
        SetupDropdown(dropdownD, ButtonsPresets.Instance.KeyCodeD, keyOptions);

        dropdownA.onValueChanged.AddListener(delegate { UpdateKeyBinding(dropdownA, "A"); });
        dropdownW.onValueChanged.AddListener(delegate { UpdateKeyBinding(dropdownW, "W"); });
        dropdownS.onValueChanged.AddListener(delegate { UpdateKeyBinding(dropdownS, "S"); });
        dropdownD.onValueChanged.AddListener(delegate { UpdateKeyBinding(dropdownD, "D"); });
    }

    private void SetupDropdown(TMP_Dropdown dropdown, KeyCode currentKey, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = options.IndexOf(currentKey.ToString());
    }

    private void UpdateKeyBinding(TMP_Dropdown dropdown, string key)
    {
        KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), dropdown.options[dropdown.value].text);
        switch (key)
        {
            case "A": ButtonsPresets.Instance.SetKeyA(newKey); break;
            case "W": ButtonsPresets.Instance.SetKeyW(newKey); break;
            case "S": ButtonsPresets.Instance.SetKeyS(newKey); break;
            case "D": ButtonsPresets.Instance.SetKeyD(newKey); break;
        }
    }
}
