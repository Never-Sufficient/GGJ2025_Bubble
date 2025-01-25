using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractCollections : MonoBehaviour
{
    [SerializeField] private float attractSpeed;
    private GameObject hook;
    private bool isCatched = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
                hook = collision.gameObject;
                hook.GetComponent<FishingRodInputHandle>().setGotCollection();
                isCatched = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, collision.gameObject.transform.position, Time.deltaTime * attractSpeed);
            }
        }
    }

}
