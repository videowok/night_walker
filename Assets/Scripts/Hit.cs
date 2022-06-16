using UnityEngine;

public class Hit : MonoBehaviour
{
    private const float DEFAULT_DURATION = .1f;
    private const float DEFAULT_SCALE_MAX = 1;

    private float age;
    private Renderer _renderer;

    protected Color color;
    protected float duration;
    protected float maxScale;


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
        _renderer = gameObject.GetComponent<Renderer>();
    }

    // POLYMORPHISM
    protected virtual void HandleDefaultUpdate()
    {
        age += Time.deltaTime;

        if (age >= duration)
        {
            Destroy(gameObject);
            return;
        }

        // scale

        float s_base = age / duration;
        float s = s_base * maxScale;
        //gameObject.transform.localScale.Set(s, s, s);    // does not work on property
        Vector3 scale = transform.localScale;
        scale.Set(s, s, s);
        gameObject.transform.localScale = scale;

        // alpha

        //color.a = (1.0f - s_base) * .5f;
        color.a = (1.0f - Mathf.Sin(s_base * (Mathf.PI / 2.0f))) * .25f;
        _renderer.material.SetColor("_Color", color);
    }

    // ABSTRACTION
    protected void SetPosAndScale(Vector3 pos)
    {
        gameObject.transform.position = pos;
        gameObject.transform.localScale.Set(0, 0, 0);
    }

    // ABSTRACTION
    public virtual void Init(Vector3 pos)
    {
        duration = DEFAULT_DURATION;
        maxScale = DEFAULT_SCALE_MAX;
        color = Color.white;

        SetPosAndScale(pos);
    }
}
