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
            Debug.LogWarning("Couldn’t load previous times for: " +
            playerName + ". Exception: " + ex.Message);
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
        if (scene.name == "Game")
        {
            DisplayPreviousTimes();
        }
    }
}
