using System;
using System.Linq;
using cfg;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using EventCenter;
using GameController;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YooAsset;

namespace ShipScene
{
    public class FishShower : MonoBehaviour
    {
        [SerializeField] private Image fishName, fish, fishCost;
        private FishCfg fishData;

        private void Awake()
        {
            EventManager.Instance.AddListener<FishCfg>(EventName.StartFishing, OnStartFishing);
            EventManager.Instance.AddListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<FishCfg>(EventName.StartFishing, OnStartFishing);
            EventManager.Instance.RemoveListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public async UniTask ShowFish(string fishNameSprite, string fishSprite, string fishCostSprite)
        {
            SoundManager.Instance.EffectPlayStr("14");
            var pathPrefix = "Assets/Arts/Sprites/Fish/";
            var package=YooAssets.GetPackage("DefaultPackage");
            fishName.sprite = package.LoadSubAssetsSync(pathPrefix + "Sprite-names").GetSubAssetObject<Sprite>(fishNameSprite);
            fish.sprite = package.LoadSubAssetsSync(pathPrefix + fishSprite).GetSubAssetObject<Sprite>(fishSprite);
            fishCost.sprite = package.LoadSubAssetsSync(pathPrefix + "Sprite-Valuetable").GetSubAssetObject<Sprite>(fishCostSprite);
            await transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
            await UniTask.WaitForSeconds(3);
            await transform.DOScale(0, 0.5f).SetEase(Ease.InSine);
        }

        private void OnStartFishing(FishCfg data)
        {
            fishData = data;
        }

        private void OnCaughtFish()
        {
            if (fishData == null)
                return;
            ShowFish(fishData.NameSpritePath, fishData.SpritePath, fishData.CostSpritePath).Forget();
            GameData.Instance.Money += fishData.Cost;
            fishData = null;
        }
    }
}