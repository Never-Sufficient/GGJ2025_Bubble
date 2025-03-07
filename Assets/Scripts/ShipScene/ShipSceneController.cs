using System;
using System.Collections.Generic;
using System.Linq;
using cfg;
using Config;
using Data;
using EventCenter;
using GameController;
using UnityEngine;
using Utils;
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

        private float spawnTimer = 0;
        private float spawnMinX, spawnMaxX, spawnMinY, spawnMaxY;
        private bool activing = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.StartOneDay, OnOneDayStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.StartOneDay, OnOneDayStart);
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
            if (activing)
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
                var (bubbleAnimName, fishCfg) = RandomChooseBubbleAndFish();
                bubble.GetComponent<Bubble>().Init(tip, bubblePool, spawnPosition, bubbleAnimName, fishCfg);
            }
        }

        private (string bubbleAnimName, FishCfg fishCfg) RandomChooseBubbleAndFish()
        {
            string bubbleAnimName = "";
            FishCfg fishCfg = null;
            //读配置表 选择Bubble类型 记录bubbleAnimName
            var randomValue = Random.Range(0, ConfigManager.Instance.Tables.TbBubbleCfg.DataList.Count);
            var bubbleCfg = ConfigManager.Instance.Tables.TbBubbleCfg.DataList[randomValue];
            bubbleAnimName = bubbleCfg.AnimName;
            //读当前数据 选择深度
            var curDepthCanReach = GameData.Instance.DepthCanReach;
            //读配置表 利用BubbleId和深度选择鱼 记录fishCfg
            var fishCfgs = (from cfg in ConfigManager.Instance.Tables.TbFishSpwanGroup.DataList
                where cfg.BubbleId == bubbleCfg.Id && cfg.ReachableDepth == curDepthCanReach
                select cfg).ToList();
            var probability = fishCfgs.Select(cfg => cfg.Probability).ToList();
            var fishIds= fishCfgs[RandomUtils.RandomSelect(probability)].FishIds;
            randomValue = Random.Range(0, fishIds.Length);
            fishCfg = ConfigManager.Instance.Tables.TbFishCfg.Get(fishIds[randomValue]);
            return (bubbleAnimName, fishCfg);
        }

        private void OnOneDayStart()
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