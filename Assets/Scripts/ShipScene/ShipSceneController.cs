using System;
using Data;
using EventCenter;
using GameController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShipScene
{
    public class ShipSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject playableArea;
        [SerializeField] private GameObject bubbleArea;
        [SerializeField] private BubblePool bubblePool;
        [SerializeField] private GameObject tip;
        [SerializeField] private float bubbleSpawnDelay;
        [SerializeField] private FishingDataSo fishingDataSo;

        private float spawnTimer = 0;
        private float spawnMinX, spawnMaxX, spawnMinY, spawnMaxY;
        private bool activing = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void Start()
        {
            spawnMaxX = bubbleArea.transform
                .TransformPoint(bubbleArea.transform.localPosition + new Vector3(0.5f, 0.5f)).x;
            spawnMaxY = bubbleArea.transform
                .TransformPoint(bubbleArea.transform.localPosition + new Vector3(0.5f, 0.5f)).y;
            spawnMinX = bubbleArea.transform
                .TransformPoint(bubbleArea.transform.localPosition + new Vector3(-0.5f, -0.5f))
                .x;
            spawnMinY = bubbleArea.transform
                .TransformPoint(bubbleArea.transform.localPosition + new Vector3(-0.5f, -0.5f))
                .y;
        }

        private void Update()
        {
            if(activing)
                BubbleSpawnCheck();
        }

        private void BubbleSpawnCheck()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= bubbleSpawnDelay)
            {
                spawnTimer -= bubbleSpawnDelay;
                var spawnPosition = new Vector2(Random.Range(spawnMinX, spawnMaxX), Random.Range(spawnMinY, spawnMaxY));
                var bubble = bubblePool.Instance.Get();
                bubble.GetComponent<Bubble>()
                    .Init(tip, bubblePool, spawnPosition, 1, 2, 0.3f, RandomChooseBubbleAndFish());
            }
        }

        private FishingDataSo.BubbleAndFishData RandomChooseBubbleAndFish()
        {
            var randomValue = Random.Range(0f, 1f);
            FishingDataSo.BubbleAndFishData bubbleAndFishData = new();
            foreach (var bubbleData in fishingDataSo.bubbleDataList)
            {
                randomValue -= bubbleData.chance;
                if (randomValue <= 0)
                {
                    bubbleAndFishData.bubbleAnimationName = bubbleData.bubbleAnimationName;
                    if (bubbleData.fishDataList.Count > 0)
                    {
                        FishingDataSo.FishData fishData;
                        do
                        {
                            var randomValue2 = Random.Range(0, bubbleData.fishDataList.Count - 1);
                            fishData = bubbleData.fishDataList[randomValue2];
                        } while (GameData.Instance.DepthCanReach < fishData.minDepth);

                        bubbleAndFishData.minDepth = fishData.minDepth;
                        bubbleAndFishData.fishSprite = fishData.fishSprite;
                        bubbleAndFishData.fishNameSprite = fishData.fishNameSprite;
                        bubbleAndFishData.fishCost = fishData.fishCost;
                        bubbleAndFishData.fishCostSprite = fishData.fishCostSprite;
                    }

                    break;
                }
            }

            return bubbleAndFishData;
        }

        private void OnGameStart()
        {
            activing = true;
        }

        private void OnTimerExpire()
        {
            activing = false;
        }

        public void SetBubbleSpawnActive(bool active)
        {
            activing = active;
        }
    }
}