using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class ShotEnemy : Shot
{
    // Start is called before the first frame update
    void Start()
    {
        HandleDefaultStart();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDefaultUpdate();
    }

    // POLYMORPHISM
    override protected void HandleCollision(Collision col)
    {
        //Debug.Log("ENEMY SHOT hit " + col.collider.name);

        base.HandleCollision(col);

        if (    col.collider.tag.Equals(Director.SHOT_PLAYER_TAG) ||
                col.collider.tag.Equals(Director.SHOT_ENEMY_TAG) ||     // enemy shots also annihilate on hit
                col.collider.tag.Equals(Director.WALL_TAG) )
        {
            Director.Instance.audioManager.Play(AudioManager.SFX.HIT_WALL);
        }
    }
}


