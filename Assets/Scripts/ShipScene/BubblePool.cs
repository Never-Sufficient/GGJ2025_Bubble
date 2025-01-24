using UnityEngine;
using UnityEngine.Pool;

namespace ShipScene
{
    public class BubblePool : MonoBehaviour
    {
        [SerializeField] private GameObject bubblePrefab;

        private IObjectPool<GameObject> instance;

        public IObjectPool<GameObject> Instance =>
            instance ??= new ObjectPool<GameObject>(CreateBubble, OnGetBubble, null, OnDestroyBubble);

        private GameObject CreateBubble()
        {
            return Instantiate(bubblePrefab, transform);
        }

        private void OnGetBubble(GameObject bubble)
        {
            bubble.SetActive(false);
        }

        private void OnDestroyBubble(GameObject bubble)
        {
            Destroy(bubble);
        }
    }
}