using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class HitMed : Hit
{
    private const float DURATION = .2f;
    private const float SCALE_MAX = 1.5f;


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
    override public void Init(Vector3 pos)
    {
        duration = DURATION;
        maxScale = SCALE_MAX;
        color = new Color(255, 255, 0, 0);

        SetPosAndScale(pos);
    }
}
