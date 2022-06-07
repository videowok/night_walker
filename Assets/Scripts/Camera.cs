using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    //private Vector3 offset = new Vector3(0, 4, -2);
    private Vector3 offset = new Vector3(0, 4, -5);
    //private float smoothness = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // default view
        transform.eulerAngles = new Vector3(30, 0, 0);
    }

    void LateUpdate()
    {
        player = GameObject.FindGameObjectWithTag("PlayerTag");

        if (player != null)
        {
            //transform.position = player.transform.position + offset;

            Vector3 v0 = player.transform.eulerAngles;
            Vector3 v1 = offset;
            v1 = Quaternion.Euler(0, v0.y, 0) * v1;
            transform.position = player.transform.position + v1;

            //v1 += player.transform.position;
            //transform.position = Vector3.Lerp(transform.position, v1, smoothness * Time.deltaTime); // Time.fixedDeltaTime);

            v1 = transform.eulerAngles;
            v1.y = v0.y;
            transform.eulerAngles = v1;
        }
    }
}
