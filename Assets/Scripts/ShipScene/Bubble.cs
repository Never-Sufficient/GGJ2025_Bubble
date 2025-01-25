using System;
using Data;
using EventCenter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private float minScale, maxScale;
        [SerializeField] private float expansionSpeed;

        private BubblePool bubblePool;
        private GameObject tip;
        private InputAction interactAction;
        private bool shipHere = false;
        private FishingDataSo.BubbleAndFishData data;

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
                Debug.Log("交互！！");
                Debug.Log(data);
                this.TriggerEvent(EventName.StartFishing, data);
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

        public void Init(GameObject tip, BubblePool bubblePool, Vector2 position, float minScale, float maxScale,
            float expansionSpeed, FishingDataSo.BubbleAndFishData bubbleAndFishData)
        {
            this.tip = tip;
            this.bubblePool = bubblePool;
            transform.position = position;
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.expansionSpeed = expansionSpeed;
            this.data = bubbleAndFishData;
            transform.localScale = new Vector3(minScale, minScale, minScale);

            gameObject.SetActive(true);
            GetComponent<Animator>().Play(data.bubbleAnimationName);
        }
    }
}