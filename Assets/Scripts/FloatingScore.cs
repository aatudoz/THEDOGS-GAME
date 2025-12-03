using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float duration = 1f;
    private TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        //poistaa objektin tietyn ajan kuluttua
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;

        //fade
        if (tmp != null)
        {
            Color color = tmp.color;
            color.a -= Time.deltaTime / duration;
            tmp.color = color;
        }
    }

    public void SetText(string text)
    {
        if (tmp != null)
        {
            tmp.text = text;
        }
    }
}
