using EventCenter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingAbilityData : MonoBehaviour
{
    [HideInInspector][Tooltip("默认滚动力大小")] public float defaultScrollForce;

    [SerializeField] private FishingAbilityDataSo fishingAbilityData;

    private void Awake()
    {
        EventManager.Instance.AddListener<int>(EventName.FishingRodLevelChanged, OnFishingAbilityLevelChanged);
    }

    private void Start()
    {
        OnFishingAbilityLevelChanged(1);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<int>(EventName.FishingRodLevelChanged, OnFishingAbilityLevelChanged);
    }

    private void OnFishingAbilityLevelChanged(int level)
    {
        var levelData = FindLevelInData(level);
        defaultScrollForce = levelData.defaultScrollForce;

    }

    private FishingAbilityDataSo.FishingAbilityLevelData FindLevelInData(int level)
    {
        return fishingAbilityData.fishingAbilityLevelDataList.Find(entry => entry.level == level);
    }
}
