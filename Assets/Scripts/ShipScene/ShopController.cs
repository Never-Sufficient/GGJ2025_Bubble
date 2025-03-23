using System;
using DG.Tweening;
using EventCenter;
using GameController;
using UnityEngine;
using UnityEngine.UI;

namespace ShipScene
{
    public class ShopController : MonoBehaviour
    {
        private bool dropdownOpen = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.GameStart, OnGameStart);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.GameStart, OnGameStart);
        }

        private void OnGameStart()
        {
            gameObject.SetActive(true);
            transform.localPosition = new Vector3(-498, 701, 0);
        }

        public void DropDownClicked()
        {
            SoundManager.Instance.EffectPlayStr("21");
            if (dropdownOpen)
                transform.DOLocalMove(new Vector3(-498, 701, 0), 0.5f).SetEase(Ease.InOutSine);
            else
                transform.DOLocalMove(new Vector3(-498, 398, 0), 0.5f).SetEase(Ease.OutBounce);
            dropdownOpen = !dropdownOpen;
        }

        public void TryUpgradeShip(GameObject caller, int level)
        {
            if (GameData.Instance.Money >= 100 && GameData.Instance.ShipLevel == level - 1)
            {
                SoundManager.Instance.EffectPlayStr("2");
                GameData.Instance.Money -= 100;
                caller.GetComponent<Image>().color = Color.white;
                GameData.Instance.ShipLevel++;
            }
        }

        public void TryUpgradeFishingLine(GameObject caller, int level)
        {
            if (GameData.Instance.Money >= 100 && GameData.Instance.DepthCanReach == level - 1)
            {
                SoundManager.Instance.EffectPlayStr("2");
                GameData.Instance.Money -= 100;
                caller.GetComponent<Image>().color = Color.white;
                GameData.Instance.DepthCanReach++;
            }
        }

        public void TryUpgradeRod(GameObject caller, int level)
        {
            if (GameData.Instance.Money >= 100 && GameData.Instance.RodLevel == level - 1)
            {
                SoundManager.Instance.EffectPlayStr("2");
                GameData.Instance.Money -= 100;
                caller.GetComponent<Image>().color = Color.white;
                GameData.Instance.RodLevel++;
            }
        }
    }
}