using System;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using EventCenter;
using GameController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class ShipController : MonoBehaviour
    {
        private static readonly int StartFish = Animator.StringToHash("StartFish");
        private static readonly int CaughtFish = Animator.StringToHash("CaughtFish");

        // [SerializeField] [Tooltip("移动力大小")] private float moveForce;
        // [SerializeField] [Tooltip("空气阻力")] private float drag;
        // [SerializeField] [Tooltip("摩擦阻力")] private float friction;
        // [SerializeField] [Tooltip("旋转力大小")] private float rotateForce;
        // [SerializeField] [Tooltip("旋转阻力大小")] private float rotateFriction;
        [SerializeField] private Transform exitPosition, enterPosition;

        private ShipData shipData;
        
        private InputAction moveAction;
        
        private Rigidbody2D rb2d;
        private SpriteRenderer sr;
        private Animator animator;
        
        private bool paused = false;
        private int animationMoving = 1; // 0：未移动 1：移动到开始 2：移动到结束
        private bool fishing = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.StartOneDay, OnGameStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimeExpire);
            EventManager.Instance.AddListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing, OnStartFish);
            EventManager.Instance.AddListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.StartOneDay, OnGameStart);
            EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTimeExpire);
            EventManager.Instance.RemoveListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing, OnStartFish);
            EventManager.Instance.RemoveListener(EventName.CaughtFish, OnCaughtFish);
        }

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            shipData = GetComponent<ShipData>();
            rb2d = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (!paused && animationMoving == 0 && !fishing)
            {
                var moveDir = moveAction.ReadValue<Vector2>();
                // RotateToDir(moveDir);
                Move(moveDir);
            }
            CheckDirection();
        }

        private void Move(Vector2 moveDir)
        {
            if (SoundManager.Instance.getEngineVolume() < 0.5f)
            {
                SoundManager.Instance.enLargeEngineVoulume();
            }
            //平移力
            rb2d.AddForce(moveDir * shipData.moveForce);
            //平移阻力
            rb2d.drag = shipData.drag;
            rb2d.AddForce(-rb2d.velocity.normalized * shipData.friction);
        }

        private void CheckDirection()
        {
            if(rb2d.velocity.magnitude < 0.1f)
                return;
            sr.flipX = rb2d.velocity.x < 0;
        }

        private async UniTaskVoid MoveToPosition(Vector2 position)
        {
            // rb2d.velocity = Vector2.zero;
            await StopMoving();
            sr.flipX = transform.position.x > position.x;
            rb2d.DOMove(position, 7f).SetEase(Ease.InOutCubic).onComplete = () =>
            {
                if (animationMoving == 1)
                    animationMoving = 0;
                GetComponent<Collider2D>().enabled = true;
            };
        }

        private async UniTask StopMoving()
        {
            SoundManager.Instance.resetEngineVolume();
            Vector2 lastVelocity = rb2d.velocity;
            while (rb2d.velocity.magnitude > 0.05f)
            {
                if (Vector2.Angle(lastVelocity, rb2d.velocity) > 90)
                    break;
                lastVelocity = rb2d.velocity;

                //平移力
                rb2d.AddForce(-rb2d.velocity.normalized * shipData.moveForce);
                //平移阻力
                rb2d.drag = shipData.drag;
                rb2d.AddForce(-rb2d.velocity.normalized * shipData.friction);
                await UniTask.WaitForFixedUpdate();
            }

            rb2d.velocity = Vector2.zero;
        }

        private void RotateToDir(Vector2 moveDir)
        {
            var nowDir = new Vector2(-Mathf.Sin(rb2d.rotation * Mathf.Deg2Rad),
                Mathf.Cos(rb2d.rotation * Mathf.Deg2Rad));
            var angle = Vector2.SignedAngle(nowDir, moveDir);
            if (moveDir.magnitude > 0.05f)
            {
                if (Mathf.Abs(angle) > rb2d.angularVelocity * rb2d.angularVelocity / 2f / shipData.rotateForce)
                    rb2d.angularVelocity += Mathf.Sign(angle) * shipData.rotateForce * Time.deltaTime;
                else
                    rb2d.angularVelocity -= Mathf.Sign(angle) * shipData.rotateForce * Time.deltaTime;
                if (Mathf.Abs(angle) < 1f)
                    rb2d.angularVelocity = 0;
            }
            else
                rb2d.AddTorque(-rb2d.angularVelocity * shipData.rotateFriction);
        }

        private void OnGameStart()
        {
            animationMoving = 1;
            GetComponent<Collider2D>().enabled = false;
            MoveToPosition(enterPosition.position).Forget();
            animator.Play("ShipSailing");
        }

        private void OnTimeExpire()
        {
            animationMoving = 2;
            GetComponent<Collider2D>().enabled = false;
            MoveToPosition(exitPosition.position).Forget();
        }

        private void OnStartFish(FishingDataSo.BubbleAndFishData data)
        {
            fishing = true;
            animator.SetTrigger(StartFish);
        }

        private void OnCaughtFish()
        {
            fishing = false;
            animator.SetTrigger(CaughtFish);
        }

        public void Ending()
        {
            paused = true;
        }

        public void EndingCaught()
        {
            FindObjectOfType<EndingEventDirector>().EndCaught();
        }

        public bool IsFishing()
        {
            return fishing;
        }

    }
}