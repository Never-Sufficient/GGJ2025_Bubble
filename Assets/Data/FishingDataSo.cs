using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [CreateAssetMenu(fileName = "FishingDataSo", menuName = "ScriptableObject/钓鱼相关数据", order = 0)]
    public class FishingDataSo : ScriptableObject
    {
        [Serializable]
        public class FishData
        {
            public int minDepth;
            public Sprite fishSprite;
            public Sprite fishNameSprite;
            public int fishCost;
        }

        [Serializable]
        public class BubbleData
        {
            public float chance;
            public string bubbleAnimationName;
            public List<FishData> fishDataList;
        }

        [Serializable]
        public class BubbleAndFishData
        {
            public string bubbleAnimationName;
            public int minDepth;
            public Sprite fishSprite;
            public Sprite fishNameSprite;
            public int fishCost;
        }

        public List<BubbleData> bubbleDataList;
    }
}