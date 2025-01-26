using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxDepthDataSo : ScriptableObject
{
    [CreateAssetMenu(fileName = "FishingAbilityDataSo", menuName = "ScriptableObject/鱼线长度升级相关数据", order = 0)]
    [Serializable]
    public class MaxDepthLevelData
    {
        [Tooltip("等级")] public int level;
        [Tooltip("最大鱼线长度")] public float maxDepth;
    }
    public List<MaxDepthLevelData> maxDepthLevelDataList;
}
