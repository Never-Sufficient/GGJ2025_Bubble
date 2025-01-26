using System;
using EventCenter;
using GifImporter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace ShipScene
{
    public class OpeningAndEndingController:MonoBehaviour
    {
        [SerializeField] private GameObject openingStart;
        [SerializeField] private GameObject opening;
        [SerializeField] private GameObject ending;
        [SerializeField] private GameObject endingEnd;
        [SerializeField] private GameObject title;
        private void Start()
        {
            SoundManager.Instance.PlaySound();
            openingStart.SetActive(true);
            opening.SetActive(false);
            ending.SetActive(false);
            endingEnd.SetActive(false);
        }

        private void Update()
        {
            if (Input.anyKeyDown && openingStart.activeSelf)
            {
                SoundManager.Instance.EffectPlayStr("1");
                openingStart.SetActive(false);
                opening.SetActive(true);
                opening.GetComponent<GifPlayer>().PlayOnce();
                opening.GetComponent<GifPlayer>().OnComplete(OnOpeningEnd);
            }
            if (Input.anyKeyDown && ending.activeSelf)
            {
                // SoundManager.Instance.EffectPlayStr("1");
                ending.SetActive(false);
                endingEnd.SetActive(true);
                endingEnd.GetComponent<GifPlayer>().PlayOnce();
                endingEnd.GetComponent<GifPlayer>().OnComplete(OnEndingEnd);
            }
        }

        private void OnOpeningEnd()
        {
            title.SetActive(false);
            opening.SetActive(false);
            this.TriggerEvent(EventName.GameStart);
        }

        public void Ending()
        {
            ending.SetActive(true);
        }

        private void OnEndingEnd()
        {
            endingEnd.SetActive(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}