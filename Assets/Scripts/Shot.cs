using UnityEngine;

public class Shot : MonoBehaviour
{

    private Vector3 velocity;

    private float speed = 5;
    private float age;


    // Start is called before the first frame update
    void Start()
    {
        age = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 dVel = velocity * Time.deltaTime;
        pos += dVel;
        gameObject.transform.position = pos;

        age += Time.deltaTime;

        if (age >= 20)      // safety
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 pos, Vector3 dir)
    {
        Vector3 radius = dir * .75f;     // let's not hit self
        gameObject.transform.position = pos + radius;

        velocity = dir * speed;
    }

    protected void ShowHit(Vector3 pos)
    {
        GameObject gob = Instantiate(Director.Instance.GetHitPrefab());
        Hit hit = gob.GetComponent<Hit>();
        hit.Init(pos);
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("SHOT hit " + col.collider.name);

        ShowHit(gameObject.transform.position);
        Destroy(gameObject);

        if (col.collider.tag.Equals("ShotTag") || col.collider.tag.Equals("WallTag"))
        {
            Director.Instance.audioManager.Play(AudioManager.SFX.HIT_WALL);
        }
    }
}
