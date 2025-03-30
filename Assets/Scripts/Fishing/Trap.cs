using EventCenter;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float trapForce;
    [SerializeField] private Sprite[] trapArray;
    // Start is called before the first frame update
    private void Awake()
    {
        EventManager.Instance.AddListener(EventName.CaughtFish, OnTrapDestroy);
        EventManager.Instance.AddListener(EventName.TimerExpire, OnTrapDestroy);
    }
    void Start()
    {
        int trapNumber = Random.Range(0, 4);
        gameObject.GetComponent<SpriteRenderer>().sprite = trapArray[trapNumber];
        if (trapNumber <= 1)
        {
            if (transform.localPosition.x > 0)
            {
                gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(Random.Range(0, 2) * 180f, 180f, 0f));
            }
        }
        else
        {
            if (transform.localPosition.x < 0)
            {
                gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(Random.Range(0, 2) * 180f, 180f, 0f));
            }
        }
        gameObject.AddComponent<PolygonCollider2D>();
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.CaughtFish, OnTrapDestroy);
        EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTrapDestroy);
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Hook")
        {
            if (collision.collider.GetComponent<FishingRodInputHandle>().getCollection == false)
            {
                SoundManager.Instance.EffectPlayStr("10");
                Vector2 direction = new Vector2(0.0f, 1.0f);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * trapForce, ForceMode2D.Impulse);

            }
            else
            {
                SoundManager.Instance.EffectPlayStr("10");
                Vector2 direction = new Vector2(0.0f, 1.0f);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-direction * trapForce, ForceMode2D.Impulse);
            }

        }
    }
    private void OnTrapDestroy()
    {
        Destroy(gameObject);
    }
    private void OnTimerExpire()
    {
        Destroy(gameObject, 5.0f);
    }
}
