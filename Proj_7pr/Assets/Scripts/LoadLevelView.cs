using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadLevelView : MonoBehaviour
{
    public TMP_Text levelNameText;

    private string levelFilePath;
    private SaveLoaderLevel saveLoaderLevel;

    public void Initialize(string levelName, string filePath, SaveLoaderLevel saveLoaderLevel)
    {
        levelNameText.text = levelName;
        levelFilePath = filePath;
        this.saveLoaderLevel = saveLoaderLevel;
    }

    public void Click()
    {
        saveLoaderLevel.LoadLevel(levelFilePath);
    }
}
