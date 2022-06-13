using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum TYPE
    {
        ISOMETRIC = 0,
        FIRST_PERSON,
        ABOVE,

        COUNT,

        DEMO
    };

    private TYPE type;  // = TYPE.ISOMETRIC;

    private Vector3[] offsetByType =
    {
        new Vector3(0, 4, -5),
        new Vector3(0, .4f, 0),
        new Vector3(0, 15, 0),
        new Vector3(0, 0, 0),   // NOT USED (COUNT)
        new Vector3(15, 7, -4)  // DEMO
    };

    private float[] angleByType = {30, 0, 90, 0, 35};  // on x (pitch)
    private Vector3 angle = new Vector3(0,0,0);


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LateUpdate()
    {
        if (type == TYPE.DEMO)  // no update in DEMO
            return;

        UpdateCamara();
    }

    private void SetRotation(float y)
    {
        angle.y = y;
        transform.eulerAngles = angle;
    }

    private void UpdateByOffset(Vector3 pos)
    {
        transform.position = pos + offsetByType[(int)type];
        SetRotation(0);
    }

    private void UpdateByRotatedOffset(GameObject player)
    {
        if (player == null)
            return;

        Vector3 v0 = player.transform.eulerAngles;
        Vector3 v1 = offsetByType[(int)type];
        v1 = Quaternion.Euler(0, v0.y, 0) * v1;
        transform.position = player.transform.position + v1;

        SetRotation(v0.y);
    }

    private void UpdateCamara()
    {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");

        switch (type)
        {
            case TYPE.ISOMETRIC:
            case TYPE.FIRST_PERSON:

                UpdateByRotatedOffset(player);
                break;

            case TYPE.ABOVE:

                UpdateByOffset(player.transform.position);
                break;

            default:    // demo

                transform.position = offsetByType[(int)TYPE.DEMO];
                SetRotation(0);
                break;
        }
    }

    public void SetType(TYPE t)
    {
        type = t;
        angle.x = angleByType[(int)type];
        UpdateCamara();
    }

    public void SwitchCamera()
    {
        if (type == TYPE.DEMO)  // no switching in DEMO
            return;

        type++;
        if (type >= TYPE.COUNT)
            type = TYPE.ISOMETRIC;

        SetType(type);
    }
}