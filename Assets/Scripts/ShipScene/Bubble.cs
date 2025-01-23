using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private float minScale, maxScale;
        [SerializeField] private float expansionSpeed;

        private GameObject tip;
        private InputAction interactAction;
        private bool shipHere = false;

        private void Start()
        {
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(maxScale, maxScale, maxScale),
                expansionSpeed * Time.deltaTime);
            if (transform.localScale.x / maxScale >= 0.95)
                gameObject.SetActive(false);

            if (interactAction.WasPressedThisFrame() && shipHere)
                Debug.Log("交互！！");
        }

        private void OnDisable()
        {
            if(shipHere)
                tip.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Ship"))
                return;
            if(tip.activeSelf)
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
            tip.SetActive(false);
        }

        public void Init(float minScale, float maxScale, float expansionSpeed,GameObject tip)
        {
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.expansionSpeed = expansionSpeed;
            this.tip = tip;
            transform.localScale = new Vector3(minScale, minScale, minScale);
        }
    }
}