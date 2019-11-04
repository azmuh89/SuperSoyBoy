using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationsPerMinute = 640f;

    void Start()
    {
        
    }
    
    void Update()
    {
        transform.Rotate(0, 0, rotationsPerMinute * Time.deltaTime, Space.Self);
    }
}
