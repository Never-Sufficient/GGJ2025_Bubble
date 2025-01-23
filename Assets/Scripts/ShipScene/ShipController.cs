using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace ShipScene
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] [Tooltip("移动力大小")] private float moveForce;
        [SerializeField] [Tooltip("空气阻力")] private float drag;
        [SerializeField] [Tooltip("摩擦阻力")] private float friction;
        [SerializeField] [Tooltip("旋转力大小")] private float rotateForce;
        [SerializeField] [Tooltip("旋转阻力大小")] private float rotateFriction;

        private InputAction moveAction;
        private Rigidbody2D rb2d;

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var moveDir = moveAction.ReadValue<Vector2>();
            // RotateToDir(moveDir);
            Move(moveDir);
        }

        private void Move(Vector2 moveDir)
        {
            //平移力
            rb2d.AddForce(moveDir * moveForce);
            //平移阻力
            rb2d.drag = drag;
            rb2d.AddForce(-rb2d.velocity.normalized * friction);
        }

        private void RotateToDir(Vector2 moveDir)
        {
            var nowDir = new Vector2(-Mathf.Sin(rb2d.rotation * Mathf.Deg2Rad),
                Mathf.Cos(rb2d.rotation * Mathf.Deg2Rad));
            var angle = Vector2.SignedAngle(nowDir, moveDir);
            if (moveDir.magnitude > 0.05f)
            {
                if (Mathf.Abs(angle) > rb2d.angularVelocity * rb2d.angularVelocity / 2f / rotateForce)
                    rb2d.angularVelocity += Mathf.Sign(angle) * rotateForce * Time.deltaTime;
                else
                    rb2d.angularVelocity -= Mathf.Sign(angle) * rotateForce * Time.deltaTime;
                if (Mathf.Abs(angle) < 1f)
                    rb2d.angularVelocity = 0;
            }
            else
                rb2d.AddTorque(-rb2d.angularVelocity * rotateFriction);
        }
    }
}