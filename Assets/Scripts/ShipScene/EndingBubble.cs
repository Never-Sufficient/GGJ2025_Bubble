using GameController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class EndingBubble:MonoBehaviour
    {
        // private GameObject tip;
        private InputAction interactAction;
        private bool shipHere = false;

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
                SoundManager.Instance.EffectPlayStr("3");
                FindObjectOfType<EndingEventDirector>().InteractedWithBubble();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Ship"))
                return;
            // if (tip.activeSelf)
            //     return;
            shipHere = true;
            // tip.transform.position = transform.position;
            // tip.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Ship"))
                return;
            shipHere = false;
            // tip?.SetActive(false);
        }
    }
}