using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventCenter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipScene
{
    public class ShipController : MonoBehaviour
    {
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
        private bool paused = false;
        private int animationMoving = 0; // 0：未移动 1：移动到开始 2：移动到结束

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimeExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTimeExpire);
        }

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            shipData = GetComponent<ShipData>();
            rb2d = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            if (!paused && animationMoving == 0)
            {
                var moveDir = moveAction.ReadValue<Vector2>();
                // RotateToDir(moveDir);
                Move(moveDir);
            }
            CheckDirection();
        }

        private void Move(Vector2 moveDir)
        {
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
        }

        private void OnTimeExpire()
        {
            animationMoving = 2;
            GetComponent<Collider2D>().enabled = false;
            MoveToPosition(exitPosition.position).Forget();
        }
    }
}