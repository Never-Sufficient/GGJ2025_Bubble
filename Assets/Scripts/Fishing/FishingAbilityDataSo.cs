using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingAbilityDataSo : ScriptableObject
{
    [CreateAssetMenu(fileName = "FishingAbilityDataSo", menuName = "ScriptableObject/钓鱼升级相关数据", order = 0)]
    [Serializable]
    public class FishingAbilityLevelData
    {
        [Tooltip("等级")] public int level;
        [Tooltip("默认滚动力大小")] public float defaultScrollForce;
    }
    public List<FishingAbilityLevelData> fishingAbilityLevelDataList;

}