using EventCenter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractCollections : MonoBehaviour
{
    [SerializeField] private float attractSpeed;
    private GameObject hook;
    private bool isCatched = false;
    // Start is called before the first frame update
    private void Awake()
    {
        EventManager.Instance.AddListener(EventName.CaughtFish, OnCollectionDestroy);
        EventManager.Instance.AddListener(EventName.TimerExpire, OnCollectionDestroy);
    }
    void Start()
    {
        
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.CaughtFish, OnCollectionDestroy);
        EventManager.Instance.RemoveListener(EventName.TimerExpire, OnCollectionDestroy);
    }
    // Update is called once per frame
    void Update()
    {
        if (isCatched)
        {
            transform.position = hook.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Hook")
        {

            if (Vector2.Distance(collision.gameObject.transform.position, transform.position) < 0.1f)
            {
                if (!isCatched)
                {
                    SoundManager.Instance.EffectPlayStr("17");
                }

                hook = collision.gameObject;
                hook.GetComponent<FishingRodInputHandle>().setGotCollection(true);
                isCatched = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, collision.gameObject.transform.position, Time.deltaTime * attractSpeed);
            }
        }
    }
    private void OnCollectionDestroy()
    {
        Destroy(gameObject);
    }
    private void OnTimerExpire()
    {
        gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        //hook.GetComponent<FishingRodInputHandle>().setGotCollection(false);
        Destroy(gameObject,5.0f);
    }
}
