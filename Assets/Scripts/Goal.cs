using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    public AudioClip goalClip;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && goalClip != null)
            {
                audioSource.PlayOneShot(goalClip);
            }

            GameManager.instance.RestartLevel(0.5f);

            // finds the times script component instance in the level scene.
            var timer = FindObjectOfType<Timer>();
            // called on the GameManager  singleton, passing in the current level runtime from the Timer script.
            GameManager.instance.SaveTime(timer.time);
        }
    }
}
