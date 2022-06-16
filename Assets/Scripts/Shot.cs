using UnityEngine;

public class Shot : MonoBehaviour
{
    private Vector3 velocity;

    private float speed = 5;
    private float age;

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
    protected virtual void HandleDefaultStart()
    {
        age = 0;
    }

    // POLYMORPHISM
    protected virtual void HandleDefaultUpdate()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 dVel = velocity * Time.deltaTime;
        pos += dVel;
        gameObject.transform.position = pos;

        age += Time.deltaTime;

        if (age >= 10)      // safety
        {
            Destroy(gameObject);
        }
    }

    // POLYMORPHISM
    public virtual void Init(Vector3 pos, Vector3 dir, AudioManager.SFX sfx)
    {
        Vector3 radius = dir * .75f;     // let's not hit self
        gameObject.transform.position = pos + radius;

        velocity = dir * speed;

        Director.Instance.audioManager.Play(sfx);
    }

    // POLYMORPHISM
    protected virtual void ShowHit(Vector3 pos)
    {
        GameObject gob = Instantiate(Director.Instance.GetHitPrefab());
        Hit hit = gob.GetComponent<Hit>();
        hit.Init(pos);
    }

    // POLYMORPHISM
    protected virtual void HandleCollision(Collision col)
    {
        //Debug.Log("SHOT hit " + col.collider.name);

        ShowHit(gameObject.transform.position);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        HandleCollision(col);
    }
}
