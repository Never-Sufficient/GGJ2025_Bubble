using System;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using EventCenter;
using GameController;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ShipScene
{
    public class FishShower : MonoBehaviour
    {
        [SerializeField] private Image fishName, fish, fishCost;
        private FishingDataSo.BubbleAndFishData fishData;

        private void Awake()
        {
            EventManager.Instance.AddListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing, OnStartFishing);
            EventManager.Instance.AddListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing,
                OnStartFishing);
            EventManager.Instance.RemoveListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public async UniTask ShowFish(Sprite fishNameSprite, Sprite fishSprite, Sprite fishCostSprite)
        {
            SoundManager.Instance.EffectPlayStr("14");
            fishName.sprite = fishNameSprite;
            fish.sprite = fishSprite;
            fishCost.sprite = fishCostSprite;
            await transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
            await UniTask.WaitForSeconds(3);
            await transform.DOScale(0, 0.5f).SetEase(Ease.InSine);
        }

        private void OnStartFishing(FishingDataSo.BubbleAndFishData data)
        {
            fishData = data;
        }

        private void OnCaughtFish()
        {
            if (fishData == null)
                return;
            ShowFish(fishData.fishNameSprite, fishData.fishSprite, fishData.fishCostSprite).Forget();
            GameData.Instance.Money += fishData.fishCost;
            fishData = null;
        }
    }
}