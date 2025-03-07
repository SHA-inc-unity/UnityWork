using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class SaveLoaderLevel : MonoBehaviour
{
    public GameObject parentPlatform;
    public GameObject boi;
    private string filePath;

    public GameObject planePrefab;
    public GameObject jumpGroundPrefab;
    public GameObject dirtPrefab;
    public GameObject movingPlatformPrefab;

    public TMP_InputField levelNameInputField;

    public GameObject loadLevelViewPanel;

    public GameObject loadLevelViewPrefab;

    void Start()
    {
        string gameFolderPath = Application.persistentDataPath;

        if (!Directory.Exists(gameFolderPath))
        {
            Directory.CreateDirectory(gameFolderPath);
        }

        CreateLevelObjectView();
    }


    public void SaveLevel()
    {
        string levelName = levelNameInputField.text;
        if (string.IsNullOrEmpty(levelName))
        {
            levelName = "defaultLevel";
        }

        filePath = Path.Combine(Application.persistentDataPath, levelName + ".json");

        boi.transform.SetParent(null);

        List<SerializedPlatform> serializedPlatforms = new List<SerializedPlatform>();

        Transform[] childTransforms = parentPlatform.GetComponentsInChildren<Transform>();

        foreach (Transform platform in childTransforms)
        {
            if (platform == parentPlatform.transform) continue;
            if (platform.parent.gameObject != parentPlatform.gameObject) continue;

            MovePlatform movePlatform = platform.GetComponent<MovePlatform>();
            if (movePlatform != null)
            {
                serializedPlatforms.Add(new SerializedPlatform(platform.gameObject, movePlatform.positions, movePlatform.speed));
            }
            else
            {
                serializedPlatforms.Add(new SerializedPlatform(platform.gameObject));
            }
        }

        string json = JsonUtility.ToJson(new LevelData(serializedPlatforms), true);
        File.WriteAllText(filePath, json);

        Debug.Log("Level Saved to " + filePath);
    }

    void CreateLevelObjectView()
    {
        foreach (Transform child in loadLevelViewPanel.transform)
        {
            Destroy(child.gameObject);
        }

        string gameFolderPath = Application.persistentDataPath;

        string[] levelFiles = Directory.GetFiles(gameFolderPath, "*.json");

        foreach (string levelFile in levelFiles)
        {
            string levelName = Path.GetFileNameWithoutExtension(levelFile);
            CreateLoadLevelView(levelName, levelFile);
        }
    }

    void CreateLoadLevelView(string levelName, string filePath)
    {
        GameObject loadLevelViewObject = Instantiate(loadLevelViewPrefab, loadLevelViewPanel.transform);
        LoadLevelView loadLevelView = loadLevelViewObject.GetComponent<LoadLevelView>();

        loadLevelView.Initialize(levelName, filePath, this);
    }

    public void LoadLevel(string levelFilePath)
    {
        if (File.Exists(levelFilePath))
        {
            boi.transform.SetParent(null);

            string json = File.ReadAllText(levelFilePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            foreach (Transform child in parentPlatform.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var platformData in levelData.platforms)
            {
                GameObject platformObject = null;

                if (platformData.tag == "Ground" && platformData.movementPositions.Count > 0)
                {
                    platformObject = Instantiate(movingPlatformPrefab);
                    var movePlatform = platformObject.GetComponent<MovePlatform>();
                    var positions = new List<Transform>();
                    foreach (var pos in platformData.movementPositions)
                    {
                        GameObject movePoint = new GameObject("MovePoint");
                        movePoint.transform.position = pos;
                        positions.Add(movePoint.transform);
                    }
                    movePlatform.Initialize(positions);
                    movePlatform.speed = platformData.speed;
                }
                else if (platformData.tag == "Jumper")
                {
                    platformObject = Instantiate(jumpGroundPrefab);
                }
                else if (platformData.tag == "Dirt")
                {
                    platformObject = Instantiate(dirtPrefab);
                }
                else if (platformData.tag == "Ground")
                {
                    platformObject = Instantiate(planePrefab);
                }
                else
                {
                    platformObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                }

                platformObject.transform.position = platformData.position;
                platformObject.transform.SetParent(parentPlatform.transform);
            }

            Debug.Log("Level Loaded from " + levelFilePath);
        }
        else
        {
            Debug.LogError("Level file not found at " + levelFilePath);
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public List<SerializedPlatform> platforms;

        public LevelData(List<SerializedPlatform> platforms)
        {
            this.platforms = platforms;
        }
    }

    [System.Serializable]
    public class SerializedPlatform
    {
        public string tag;
        public Vector3 position;
        public List<Vector3> movementPositions;
        public float speed;

        public SerializedPlatform(GameObject platform)
        {
            tag = platform.tag;
            position = platform.transform.position;
            movementPositions = new List<Vector3>();
            speed = 0;
        }

        public SerializedPlatform(GameObject platform, List<Transform> positions, float speed)
        {
            tag = platform.tag;
            position = platform.transform.position;
            movementPositions = new List<Vector3>();

            foreach (var pos in positions)
            {
                movementPositions.Add(pos.position);
            }

            this.speed = speed;
        }
    }
}
