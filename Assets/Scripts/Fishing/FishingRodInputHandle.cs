using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRodInputHandle : MonoBehaviour
{
    [SerializeField] private Transform lineStartPosition;
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject backGround;
    [SerializeField] private LineRenderer fishingLine;
    [SerializeField] private float defaultScrollForce;
    [SerializeField] private float variableScrollForce;
    [SerializeField] private float maxScrollForce;
    [SerializeField] private float scrollForceGrowingSpeedIntensity;
    [SerializeField] private float horizontalMoveIntensity;
    [SerializeField] private float drag;
    [SerializeField] private float friction;
    private InputAction mouseAction;
    private float lastVelocity = 0;
    private bool hookAtTop = false;
    private bool hookAtMid = false;
    private bool hookAtBottom = false;
    //private bool bgAtTop = true;
    //private bool bgAtBottom = false;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    public float currentDepth { get; private set; }
    public float maxDepth {  get; private set; }
    
    private void Start()
    {
        mouseAction = InputSystem.actions.FindAction("MouseOperation");
        variableScrollForce = defaultScrollForce;
    }

    /*private void FixedUpdate()
    {
        Vector2 MouseOperationDir = mouseAction.ReadValue<Vector2>();
        Debug.Log("hookAtMid:" + hookAtMid);
        if (hookAtTop||hookAtMid)
        {
            HookScroll(MouseOperationDir);
            HorizontalMove(MouseOperationDir);
        }
        if ((hookAtTop && MouseOperationDir.y > 0) || (hookAtBottom && MouseOperationDir.y < 0))
        {
            if (backGround.transform.position.y <= topPoint.position.y && backGround.transform.position.y >= bottomPoint.position.y){
                BackGroundScroll(MouseOperationDir);
                HorizontalMove(MouseOperationDir);
                Debug.Log(2);
                //currentDepth = Mathf.Abs(bottomPoint.position.y - hook.transform.position.y) / Mathf.Abs(topPoint.position.y - bottomPoint.transform.position.y) * maxDepth;
            }
        }
        DrawFishingLine();
        GetHookPositionOnScreen();
        BackgroundEdgeRegress();
    }*/
    private void FixedUpdate()
    {
        Vector2 MouseOperationDir = mouseAction.ReadValue<Vector2>();
        BackGroundScroll(MouseOperationDir);
        HorizontalMove(MouseOperationDir);
        DrawFishingLine();

    }
    private void GetHookPositionOnScreen()
    {
        Vector2 hookPositionOnScreen = Camera.main.WorldToScreenPoint(hook.transform.position);
        if( hookPositionOnScreen.y > Screen.height * 0.666f)
        {
            hookAtTop = true;
            hookAtMid = false;
            hookAtBottom = false;
        }
        if (hookPositionOnScreen.y < Screen.height * 0.666f && hookPositionOnScreen.y > Screen.height * 0.333f)
        {
            hookAtTop = false;
            hookAtMid = true;
            hookAtBottom = false;
        }
        if (hookPositionOnScreen.y < Screen.height * 0.333f)
        {
            hookAtTop = false;
            hookAtMid = false;
            hookAtBottom = true;
        }
    }
    private void BackgroundEdgeRegress()
    {
        if(backGround.transform.position.y < bottomPoint.transform.position.y)
        {
            backGround.transform.position = bottomPoint.transform.position;
        }
        if(backGround.transform.position.y > topPoint.transform.position.y)
        {
            backGround.transform.position = topPoint.transform.position;
        }
    }
    private void HookScroll(Vector2 MouseOperationDir)
    {
        Rigidbody2D rb2d = hook.GetComponent<Rigidbody2D>();
        float currentVelocity = rb2d.velocity.normalized.y;
        if (currentVelocity != lastVelocity)
        {
            variableScrollForce = defaultScrollForce;
        }
        if (rb2d.velocity.normalized.y != 0)
        {
            if (variableScrollForce < maxScrollForce)
            {
                variableScrollForce += scrollForceGrowingSpeedIntensity;
            }
        }
        rb2d.AddForce(new Vector2(0.0f, MouseOperationDir.y) * variableScrollForce, ForceMode2D.Impulse);
        rb2d.drag = drag;
        rb2d.AddForce(-rb2d.velocity.normalized * friction);
        lastVelocity = currentVelocity;
    }
    private void BackGroundScroll(Vector2 MouseOperationDir)
    {
        Rigidbody2D rb2d = backGround.GetComponent<Rigidbody2D>();
        float currentVelocity = rb2d.velocity.normalized.y;
        if(currentVelocity != lastVelocity)
        {
            variableScrollForce = defaultScrollForce;
        }
        if (rb2d.velocity.normalized.y != 0 )
        {
            if (variableScrollForce < maxScrollForce)
            {
                variableScrollForce += scrollForceGrowingSpeedIntensity;
            }
        }
        rb2d.AddForce(-new Vector2(0.0f, MouseOperationDir.y) * variableScrollForce, ForceMode2D.Impulse);
        rb2d.drag = drag;
        rb2d.AddForce(-rb2d.velocity.normalized * friction);
        lastVelocity = currentVelocity;
    }
    private void HorizontalMove(Vector2 MouseOperationDir)
    {
        Transform transform = hook.transform;
        //transform.position = new Vector3(transform.position.x + MouseOperationDir.x * horizontalMoveIntensity, transform.position.y, transform.position.z);
        transform.Translate(MouseOperationDir.x * horizontalMoveIntensity, 0.0f, 0.0f);
    }
    private void DrawFishingLine()
    {
        Vector3[] points = new Vector3[2] { lineStartPosition.position, hook.transform.position };
        fishingLine.positionCount = 2;
        fishingLine.SetPositions(points);
    }
    public void setTopPoint(Vector2 Point)
    {
        topPoint.position = Point;
    }
    public void setBottomPoint(Vector2 Point)
    {
        bottomPoint.position = Point;
    }
    public void setMaxDepth(float depth)
    {
        maxDepth = depth;
    }
}
