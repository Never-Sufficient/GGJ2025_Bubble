using System;
using EventCenter;
using GifImporter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace ShipScene
{
    public class OpeningAndEndingController:MonoBehaviour
    {
        [SerializeField] private GameObject openingStart;
        [SerializeField] private GameObject opening;
        [SerializeField] private GameObject ending;
        [SerializeField] private GameObject endingEnd;
        private void Start()
        {
            openingStart.SetActive(true);
            opening.SetActive(false);
            ending.SetActive(false);
            endingEnd.SetActive(false);
        }

        private void Update()
        {
            if (Input.anyKeyDown && openingStart.activeSelf)
            {
                openingStart.SetActive(false);
                opening.SetActive(true);
                opening.GetComponent<GifPlayer>().PlayOnce();
                opening.GetComponent<GifPlayer>().OnComplete(OnOpeningEnd);
            }
        }

        public void OnOpeningEnd()
        {
            opening.SetActive(false);
            this.TriggerEvent(EventName.GameStart);
        }
    }
}