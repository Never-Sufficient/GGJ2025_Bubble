using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float trapForce;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Hook")
        {
            if(collision.GetComponent<FishingRodInputHandle>().getCollection == false)
            {
                Vector2 direction = new Vector2(0.0f, 1.0f);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * trapForce, ForceMode2D.Impulse);
            }
            
        }
    }
}
