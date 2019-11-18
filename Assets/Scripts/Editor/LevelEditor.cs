using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Save level"))
        {
        }

        // grabs a handle on the Level component and sets the Level Gameobject's position and rotation to 0.
        Level level = (Level)target;
        level.transform.position = Vector3.zero;
        level.transform.rotation = Quaternion.identity;
        // establishes levelRoot as a reference to the Level GameObject
        var levelRoot = GameObject.Find("Level");
        // creates new instances of LevelDataRepresentation and LevelItemRepresentation
        var ldr = new LevelDataRepresentation();
        var levelItems = new List<LevelItemRepresentation>();

        foreach (Transform t in levelRoot.transform)
        {
            // loops through every child Transform object of levelRoot.
            var sr = t.GetComponent<SpriteRenderer>();
            var li = new LevelItemRepresentation()
            {
                position = t.position,
                rotation = t.rotation.eulerAngles,
                scale = t.localScale
            };

            // bit of a hack to store the name of each level item in the prefabName field.
            if (t.name.Contains(" "))
            {
                li.prefabName = t.name.Substring(0, t.name.IndexOf(" "));
            }
            else
            {
                li.prefabName = t.name;
            }

            // Performs a check t ensure that a valid SpriteRenderer was found
            if (sr != null)
            {
                li.spriteLayer = sr.sortingLayerName;
                li.spriteColor = sr.color;
                li.spriteOrder = sr.sortingOrder;
            }

            // the item that has all the collected information is added to the LevelItems list
            levelItems.Add(li);

            // converts the entire list of level items to an array of LevelItemRepresentation objects and stores them in the LevelItems field on the LevelDataRepresentation object.
            ldr.levelItems = levelItems.ToArray();
            ldr.playerStartPosition =
             GameObject.Find("SoyBoy").transform.position;
            // Locates the CameraLerpToTranform script in the scene then maps its settings against a new CameraSettingsRepresentation object.
            var currentCamSettings = FindObjectOfType<CameraLerpToTransform>();

            if (currentCamSettings != null)
            {
                ldr.cameraSettings = new CameraSettingsRepresentation()
                {
                    cameraTrackTarget = currentCamSettings.camTarget.name,
                    cameraZDepth = currentCamSettings.cameraZDepth,
                    minX = currentCamSettings.minX,
                    minY = currentCamSettings.minY,
                    maxX = currentCamSettings.maxX,
                    maxY = currentCamSettings.maxY,
                    trackingSpeed = currentCamSettings.trackingSpeed
                };
            }

            var levelDataToJson = JsonUtility.ToJson(ldr);
            var savePath = System.IO.Path.Combine(Application.dataPath, level.levelName + ".json");
            System.IO.File.WriteAllText(savePath, levelDataToJson);
            Debug.Log("Level saved to " + savePath);
        }
    }
}