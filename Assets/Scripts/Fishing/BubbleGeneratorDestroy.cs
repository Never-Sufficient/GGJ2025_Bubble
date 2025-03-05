using EventCenter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGeneratorDestroy : MonoBehaviour
{
    private BoxCollider2D BubbleBurst;
    private void Awake()
    {
        BubbleBurst = GameObject.Find("BubbleBurst").GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.trigger.AddCollider(BubbleBurst);
        EventManager.Instance.AddListener(EventName.CaughtFish, OnCollectionDestroy);
        EventManager.Instance.AddListener(EventName.TimerExpire, OnCollectionDestroy);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.CaughtFish, OnCollectionDestroy);
        EventManager.Instance.RemoveListener(EventName.TimerExpire, OnCollectionDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollectionDestroy()
    {
        Destroy(gameObject);
    }
    private void OnTimerExpire()
    {
        Destroy(gameObject, 5.0f);
    }
}
