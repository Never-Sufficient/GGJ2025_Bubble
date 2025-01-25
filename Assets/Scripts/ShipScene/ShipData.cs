using System;
using Data;
using EventCenter;
using UnityEngine;

namespace ShipScene
{
    public class ShipData : MonoBehaviour
    {
        [HideInInspector] [Tooltip("移动力大小")] public float moveForce;
        [HideInInspector] [Tooltip("空气阻力")] public float drag;
        [HideInInspector] [Tooltip("摩擦阻力")] public float friction;
        [HideInInspector] [Tooltip("旋转力大小")] public float rotateForce;
        [HideInInspector] [Tooltip("旋转阻力大小")] public float rotateFriction;

        [SerializeField] private ShipDataSo shipData;

        private void Awake()
        {
            EventManager.Instance.AddListener<int>(EventName.ShipLevelChanged, OnShipLevelChanged);
        }

        private void Start()
        {
            OnShipLevelChanged(1);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<int>(EventName.ShipLevelChanged, OnShipLevelChanged);
        }

        private void OnShipLevelChanged(int level)
        {
            var levelData = FindLevelInData(level);
            moveForce = levelData.moveForce;
            drag = levelData.drag;
            friction = levelData.friction;
            rotateForce = levelData.rotateForce;
            rotateFriction = levelData.rotateFriction;
        }

        private ShipDataSo.ShipLevelData FindLevelInData(int level)
        {
            return shipData.shipLevelDataList.Find(entry => entry.level == level);
        }
    }
}