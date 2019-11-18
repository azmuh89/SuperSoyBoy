using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public string playerName;
    public static GameManager instance;
    public GameObject buttonPrefab;
    private string selectedLevel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DiscoverLevels();
	}

    public void RestartLevel(float delay)
    {
        StartCoroutine(RestartLevelDelay(delay));
    }

    private IEnumerator RestartLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update () {
	
	}

    public List<PlayerTimeEntry> LoadPreviousTimes()
    {
        // use try/catch statement to attempt to load saved time entries for player
        try
        {
            var scoresFile = Application.persistentDataPath +
            "/" + playerName + "_times.dat";
            using (var stream = File.Open(scoresFile, FileMode.Open))
            {
                var bin = new BinaryFormatter();
                var times = (List<PlayerTimeEntry>)bin.Deserialize(stream);
                return times;
            }
        }

        // if deserialization is unsuccessful the catch statement finds the errors
        catch (IOException ex)
        {
            Debug.LogWarning("Couldn’t load previous times for: " + playerName + ". Exception: " + ex.Message);
            return new List<PlayerTimeEntry>();
        }
    }

    public void SaveTime(decimal time)
    {
        // when saving time, you fetch existing times first with the LoadPrevioursTimes() method.
        var times = LoadPreviousTimes();
        // create an instance of new PlayerTImeEntry object.
        var newTime = new PlayerTimeEntry();
        newTime.entryDate = DateTime.Now;
        newTime.time = time;
        // create a binary formatter object to do the magic serialization
        var bFormatter = new BinaryFormatter();
        var filePath = Application.persistentDataPath +
        "/" + playerName + "_times.dat";
        using (var file = File.Open(filePath, FileMode.Create))
        {
            times.Add(newTime);
            bFormatter.Serialize(file, times);
        }
    }

    public void DisplayPreviousTimes()
    {
        // collects existing times using the LoadPreviousTimes() method.
        var times = LoadPreviousTimes();
        var topThree = times.OrderBy(time => time.time).Take(3);
        // Find the PreviousTimes component
        var timesLabel = GameObject.Find("PreviousTimes").GetComponent<Text>();
        // Changes it to show each time found, seperating entries with a line break using the "\n" string.
        timesLabel.text = "BEST TIMES \n";
        foreach (var time in topThree)
        {
            timesLabel.text += time.entryDate.ToShortDateString() +
            ": " + time.time + "\n";
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadsceneMode)
    {
        if (!string.IsNullOrEmpty(selectedLevel) && scene.name == "Game")
        {
            Debug.Log("Loading level content for: " + selectedLevel);
            LoadLevelContent();
            DisplayPreviousTimes();
        }

        if (scene.name == "Menu")
        {
            DiscoverLevels();
        }
    }

    private void SetLevelName(string levelFilePath)
    {
        selectedLevel = levelFilePath;
        SceneManager.LoadScene("Game");
    }

    private void DiscoverLevels()
    {
        var levelPanelRectTransform =
        GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>();
        var levelFiles = Directory.GetFiles(Application.dataPath, "*.json");

        var yOffset = 0f;

        for (var i = 0; i < levelFiles.Length; i++)
        {
            if (i == 0)
            {
                yOffset = -30f;
            }
            else
            {
                yOffset -= 65f;
            }
            var levelFile = levelFiles[i];
            var levelName = Path.GetFileName(levelFile);

            // instantiates a copy of the button prefab
            var levelButtonObj = (GameObject)Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            // gets its Transform and makes it a child of LevelItemsPanel
            var levelButtonRectTransform = levelButtonObj.GetComponent<RectTransform>();
            levelButtonRectTransform.SetParent(levelPanelRectTransform, true);
            // positions it based on a fixed X-position and a variable Y-position
            levelButtonRectTransform.anchoredPosition = new Vector2(212.5f, yOffset);
            // sets the button text to the level's name.
            var levelButtonText = levelButtonObj.transform.GetChild(0).GetComponent<Text>();
            levelButtonText.text = levelName;

            var levelButton = levelButtonObj.GetComponent<Button>();
            levelButton.onClick.AddListener(
             delegate { SetLevelName(levelFile); });
            levelPanelRectTransform.sizeDelta =
             new Vector2(levelPanelRectTransform.sizeDelta.x, 60f * i);
        }

        levelPanelRectTransform.offsetMax = new Vector2(levelPanelRectTransform.offsetMax.x, 0f);
    }

    private void LoadLevelContent()
    {
        var existingLevelRoot = GameObject.Find("Level");
        Destroy(existingLevelRoot);
        var levelRoot = new GameObject("Level");

        // reads the JSON file content of the selected level
        var levelFileJsonContent = File.ReadAllText(selectedLevel);
        var levelData = JsonUtility.FromJson<LevelDataRepresentation>(
         levelFileJsonContent);
        // makes levelData.levelItems into a fully populated array of LevelItemRepresentation instances.
        foreach (var li in levelData.levelItems)
        {
            // for every item that is looped through the array, the script locates correct prefab and loads it.
            var pieceResource = Resources.Load("Prefabs/" + li.prefabName);

            if (pieceResource == null)
            {
                Debug.LogError("Cannot find resource: " + li.prefabName);
            }

            // instantiates a clone of this prefab
            var piece = (GameObject)Instantiate(pieceResource, li.position, Quaternion.identity);
            var pieceSprite = piece.GetComponent<SpriteRenderer>();

            if (pieceSprite != null)
            {
                pieceSprite.sortingOrder = li.spriteOrder;
                pieceSprite.sortingLayerName = li.spriteLayer;
                pieceSprite.color = li.spriteColor;
            }

            // makes the object a child of the Level GameObject then sets its position, rotation and scale.
            piece.transform.parent = levelRoot.transform;
            piece.transform.position = li.position;
            piece.transform.rotation = Quaternion.Euler(
            li.rotation.x, li.rotation.y, li.rotation.z);
            piece.transform.localScale = li.scale;
        }

        var SoyBoy = GameObject.Find("SoyBoy");
        SoyBoy.transform.position = levelData.playerStartPosition;
        Camera.main.transform.position = new Vector3(SoyBoy.transform.position.x, SoyBoy.transform.position.y, Camera.main.transform.position.z);

        // locates the smooth follow script CameraLerpToTransform with the FindObjectOfType() method
        var camSettings = FindObjectOfType<CameraLerpToTransform>();

        // checks that the Smooth Follow script was found, and if so, it populated settings for speed, bounds and tracking target.
        if (camSettings != null)
        {
            camSettings.cameraZDepth =
            levelData.cameraSettings.cameraZDepth;
            camSettings.camTarget = GameObject.Find(
            levelData.cameraSettings.cameraTrackTarget).transform;
            camSettings.maxX = levelData.cameraSettings.maxX;
            camSettings.maxY = levelData.cameraSettings.maxY;
            camSettings.minX = levelData.cameraSettings.minX;
            camSettings.minY = levelData.cameraSettings.minY;
            camSettings.trackingSpeed =
            levelData.cameraSettings.trackingSpeed;
        }
    }
}
