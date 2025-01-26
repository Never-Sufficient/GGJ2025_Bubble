using EventCenter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxDepthData : MonoBehaviour
{
    [HideInInspector][Tooltip("最大鱼线长度")] public float maxDepth;

    [SerializeField] private MaxDepthDataSo maxDepthData;

    private void Awake()
    {
        EventManager.Instance.AddListener<int>(EventName.MaxDepthLeveChanged, OnMaxDepthLevelChanged);
    }

    private void Start()
    {
        OnMaxDepthLevelChanged(1);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<int>(EventName.MaxDepthLeveChanged, OnMaxDepthLevelChanged);
    }

    private void OnMaxDepthLevelChanged(int level)
    {
        var levelData = FindLevelInData(level);
        maxDepth = levelData.maxDepth;

    }

    private MaxDepthDataSo.MaxDepthLevelData FindLevelInData(int level)
    {
        return maxDepthData.maxDepthLevelDataList.Find(entry => entry.level == level);
    }
}
