using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public GameObject playerDeathPrefab;
    public AudioClip deathClip;
    public Sprite hitSprite;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //check to ensure colliding GameObject has "Player" tag
        if (coll.transform.tag == "Player")
        {
            //determine if AudioCLip his been assigned and that a valid Audio Source component exists to play deathClip audio effect
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && deathClip != null)
            {
                audioSource.PlayOneShot(deathClip);
            }

            //swap the sprite of the saw blade with the hitSprite version
            Instantiate(playerDeathPrefab, coll.contacts[0].point, Quaternion.identity);
            spriteRenderer.sprite = hitSprite;

            //Destroy the colliding object (player)
            Destroy(coll.gameObject);
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
