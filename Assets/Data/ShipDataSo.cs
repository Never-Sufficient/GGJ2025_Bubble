using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ShipDataSo", menuName = "ScriptableObject/船只升级相关数据", order = 0)]
    public class ShipDataSo : ScriptableObject
    {
        [Serializable]
        public class ShipLevelData
        {
            [Tooltip("等级")] public int level;
            [Tooltip("移动力大小")] public float moveForce;
            [Tooltip("空气阻力")] public float drag;
            [Tooltip("摩擦阻力")] public float friction;
            [Tooltip("旋转力大小")] public float rotateForce;
            [Tooltip("旋转阻力大小")]public float rotateFriction;
        }

        public List<ShipLevelData> shipLevelDataList;
    }
}