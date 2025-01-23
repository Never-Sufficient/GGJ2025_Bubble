using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShipScene
{
    public class ShipSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject playableArea;
        [SerializeField] private GameObject bubblePrefab;
        [SerializeField] private GameObject tip;
        [SerializeField] private float bubbleSpawnDelay;

        private float spawnTimer = 0;
        private float spawnMinX, spawnMaxX, spawnMinY, spawnMaxY;

        private void Start()
        {
            spawnMaxX = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[0]).x;
            spawnMaxY = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[0]).y;
            spawnMinX = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[2]).x;
            spawnMinY = playableArea.transform.TransformPoint(playableArea.GetComponent<EdgeCollider2D>().points[2]).x;
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
                var bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
                bubble.GetComponent<Bubble>().Init(1, 2, 0.3f, tip);
            }
        }
    }
}