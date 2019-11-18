using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    private InputField input;

    void Start()
    {
        // locating and caching InputField component
        input = GetComponent<InputField>();
        input.onValueChanged.AddListener(SavePlayerName);
        // use PlayerPrefs to look for an retrieve value for a key named PlayerName
        var savedName = PlayerPrefs.GetString("PlayerName");
        if (!string.IsNullOrEmpty(savedName))
        {
            input.text = savedName;
            GameManager.instance.playerName = savedName;
        }
    }

    private void SavePlayerName(string playerName)
    {
        // takes the supplied playerName and sets the key named PlayerName to this value
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        GameManager.instance.playerName = playerName;
    }
}
