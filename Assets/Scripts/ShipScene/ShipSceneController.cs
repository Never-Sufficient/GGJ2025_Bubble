using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShipScene
{
    public class ShipSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject playableArea;
        [SerializeField] private BubblePool bubblePool;
        [SerializeField] private GameObject tip;
        [SerializeField] private float bubbleSpawnDelay;

        private float spawnTimer = 0;
        private float spawnMinX, spawnMaxX, spawnMinY, spawnMaxY;

        private void Start()
        {
            spawnMaxX = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[0]).x;
            spawnMaxY = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[0]).y;
            spawnMinX = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[2]).x;
            spawnMinY = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[2]).y;
        }

        private void Update()
        {
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
                bubble.GetComponent<Bubble>().Init(tip, bubblePool, spawnPosition, 1, 2, 0.3f);
            }
        }
    }
}