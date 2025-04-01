using System;
using cfg;
using Data;
using EventCenter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class Bubble : MonoBehaviour
    {
        private BubblePool bubblePool;
        private GameObject tip;
        private InputAction interactAction;
        private bool shipHere = false;
        private FishCfg data;
        private bool hasGenerated = false;

        private void Start()
        {
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        private void Update()
        {
            // transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(maxScale, maxScale, maxScale),
            //     expansionSpeed * Time.deltaTime);
            // if (transform.localScale.x / maxScale >= 0.95)
            //     BubbleExplode();

            if (interactAction.WasPressedThisFrame() && shipHere)
            {
                if (!hasGenerated)
                {
                    SoundManager.Instance.EffectPlayStr("3");
                    this.TriggerEvent(EventName.StartFishing, data);
                    hasGenerated = true;
                }

            }
        }

        public void BubbleExplode()
        {
            gameObject.SetActive(false);
            if (shipHere)
                tip.SetActive(false);
            bubblePool.Instance.Release(gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Ship"))
                return;
            if (tip.activeSelf)
                return;
            if (other.gameObject.GetComponent<ShipController>().IsFishing())
                return;
            shipHere = true;
            tip.transform.position = transform.position;
            tip.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Ship"))
                return;
            shipHere = false;
            tip?.SetActive(false);
        }

        public void Init(GameObject tip, BubblePool bubblePool, Vector2 position, string bubbleAnimationName,
            FishCfg fishCfg)
        {
            this.tip = tip;
            this.bubblePool = bubblePool;
            transform.position = position;
            this.data = fishCfg;

            gameObject.SetActive(true);
            GetComponent<Animator>().Play(bubbleAnimationName);
        }
    }
}