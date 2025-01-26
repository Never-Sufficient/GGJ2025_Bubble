using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        }
    }
}
