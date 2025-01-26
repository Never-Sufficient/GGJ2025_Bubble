using Cysharp.Threading.Tasks;
using DG.Tweening;
using ShipScene;
using UnityEngine;
using UnityEngine.UI;

namespace GameController
{
    public class EndingEventDirector : MonoBehaviour
    {
        [SerializeField] private GameObject shop, money;
        [SerializeField] private ShipSceneController shipSceneController;
        [SerializeField] private GameObject ship;
        [SerializeField] private OpeningAndEndingController openingAndEndingController;
        [SerializeField] private BubblePool bubblePool;
        [SerializeField] private Transform endingBubblePosition;
        [SerializeField] private GameObject endingBubble;
        [SerializeField] private Camera shipCamera;
        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private Image dark;

        private bool interactedWithBubble = false;
        private bool endCaught = false;

        public async UniTask StartEndingEvent()
        {
            gameTimer.enabled = false;
            shipSceneController.SetBubbleSpawnActive(false);
            Instantiate(endingBubble, endingBubblePosition.position, Quaternion.identity).SetActive(true);
            await UniTask.WaitUntil(() => interactedWithBubble);
            shop.SetActive(false);
            money.SetActive(false);
            ship.GetComponent<Collider2D>().enabled = false;
            ship.GetComponent<ShipController>().Ending();
            ship.GetComponent<Animator>().Play("EndCatch");
            await UniTask.WaitUntil(() => endCaught);
            ship.GetComponent<Animator>().Play("EndLoop");
            SpawnBubble().Forget();
            CameraAnim();
            await UniTask.WaitForSeconds(10);
            await dark.DOFade(1, 5f);
            openingAndEndingController.Ending();
            await dark.DOFade(0, 5f);
        }

        private async UniTask SpawnBubble()
        {
            while (true)
            {
                Vector3 randomVector = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                Instantiate(endingBubble, endingBubblePosition.position + randomVector, Quaternion.identity);
                await UniTask.WaitForSeconds(0.2f);
            }
        }

        public void InteractedWithBubble()
        {
            interactedWithBubble = true;
        }

        public void EndCaught()
        {
            endCaught = true;
        }

        private void CameraAnim()
        {
            shipCamera.DORect(new Rect(0, 0, 1, 1), 10);
            shipCamera.DOOrthoSize(2, 10);
            shipCamera.transform.DOMove(
                new Vector3(endingBubblePosition.position.x, endingBubblePosition.position.y, -10), 10);
        }
    }
}