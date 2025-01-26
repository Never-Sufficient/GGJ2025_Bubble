using Data;
using EventCenter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRodInputHandle : MonoBehaviour
{
    [SerializeField] private GameObject collectionPrefab;
    [SerializeField] private GameObject hook; //底部鱼钩
    [SerializeField] private GameObject backGround; //可移动背景
    [SerializeField] private float variableScrollForce; //可变化力
    [SerializeField] private float maxScrollForce; //力的最大值
    [SerializeField] private float scrollForceGrowingSpeedIntensity; //力的变化速率
    [SerializeField] private float horizontalMoveIntensity; //水平移动强度
    [SerializeField] private Transform enterWaterPosition; //下降到水底的指定位置
    [SerializeField] private Transform exitWaterPosition; //上升到水面的指定位置
    [SerializeField] private float drag;
    [SerializeField] private float friction;
    [SerializeField] private Transform[] collectionGenerateMinDepthArray;
    [SerializeField] private Transform[] topPointArray;
    [SerializeField] private Transform[] bottomPointArray;
    [SerializeField] private float[] maxDepthArray;
    private InputAction hookScrollAction, hookHorizontalMoveAction;
    private float lastVelocity = 0;
    private bool hookAtTop = false; //鱼钩顶部区域检测
    private bool hookAtMid = false; //鱼钩中间区域检测
    private bool hookAtBottom = false; //鱼钩底部区域检测
    public bool getCollection { get; private set; } = false; //钓到物品
    private bool hookEnterWater = false; //鱼钩正在入水状态检测，须外部初始化
    private bool hookExitWater = false; //鱼钩正在出水状态检测
    private bool isFishing = false;
    private Vector2 topPoint; //地图的可移动的顶部最大值，须外部初始化
    private Vector2 bottomPoint; //地图的可移动的低部最大值，须外部初始化
    public float currentDepth { get; private set; } //当前最大深度
    public float maxDepth { get; private set; } //UI最大深度，须外部初始化

    private float accumulatedScroll; //当前帧累计滚动距离
    private float accumulatedMove; //当前帧累计水平移动距离
    private FishingAbilityData fishingAbilityData;

    private void Awake()
    {
        EventManager.Instance.AddListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing, setHookEnterWater);
        EventManager.Instance.AddListener(EventName.TimerExpire, HookExitWater);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<FishingDataSo.BubbleAndFishData>(EventName.StartFishing, setHookEnterWater);
        EventManager.Instance.RemoveListener(EventName.TimerExpire, HookExitWater);
    }
    private void Start()
    {
        topPoint = Vector2.zero;
        bottomPoint = Vector2.zero;
        fishingAbilityData = GetComponent<FishingAbilityData>();
        hookScrollAction = InputSystem.actions.FindAction("HookScroll");
        hookHorizontalMoveAction = InputSystem.actions.FindAction("HookHorizontalMove");
        variableScrollForce = fishingAbilityData.defaultScrollForce;
    }

    private void Update()
    {
        accumulatedScroll += hookScrollAction.ReadValue<float>();
        accumulatedMove += hookHorizontalMoveAction.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        // Debug.Log("getCollection" + getCollection);
        // Debug.Log("hookExitWater" + hookExitWater);
        // Debug.Log(backGround.transform.position.y > topPoint.transform.position.y);

        if (hookEnterWater)
        {
            HookEnterWater();
            Invoke("delayMusic1", 0.5f);
        }

        if (hookExitWater && getCollection)
        {
            HookExitWater();
        }

        if (isFishing)
        {
            var hookScroll = accumulatedScroll;
            var hookHorizontalMove = accumulatedMove;
            accumulatedScroll = 0;
            accumulatedMove = 0;
            if (hookAtMid)
            {
                backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                HookScroll(hookScroll);
            }

            if (hookAtBottom && hookScroll > 0)
            {
                backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                HookScroll(hookScroll);
            }

            if (hookAtTop && hookScroll < 0)
            {
                backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                HookScroll(hookScroll);
            }

            if ((hookAtTop && hookScroll > 0) || (hookAtBottom && hookScroll < 0))
            {
                if (backGround.transform.position.y <= topPoint.y &&
                    backGround.transform.position.y >= bottomPoint.y)
                {
                    BackGroundScroll(hookScroll);
                    //currentDepth = Mathf.Abs(bottomPoint.position.y - hook.transform.position.y) / Mathf.Abs(topPoint.position.y - bottomPoint.transform.position.y) * maxDepth;
                }
            }

            HorizontalMove(hookHorizontalMove);
            GetHookPositionOnScreen();
            BackgroundEdgeRegress();
        }
    }

    private void HookEnterWater()
    {
        this.TriggerEvent(EventName.GamePause);
        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(enterWaterPosition.position.x,
                Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.666f,
                    hook.transform.position.z)).y), Time.deltaTime * 2.0f);
        if (Vector2.Distance(
                new Vector2(enterWaterPosition.position.x,
                    Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.666f,
                        hook.transform.position.z)).y), transform.position) < 0.01f)
        {
            transform.position = new Vector2(transform.position.x,enterWaterPosition.position.y);
            hookEnterWater = false;
            isFishing = true;
        }
    }

    private void HookExitWater()
    {
        isFishing = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = Vector2.MoveTowards(transform.position, exitWaterPosition.position, Time.deltaTime * 2.0f);
        if (Vector2.Distance(exitWaterPosition.position, transform.position) < 0.01f)
        {
            hook.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            transform.position = new Vector2(transform.position.x, exitWaterPosition.position.y);
            hookExitWater = false;
            getCollection = false;
            this.TriggerEvent(EventName.CaughtFish);
        }
    }

    private void GetHookPositionOnScreen() //鱼钩屏幕位置检测
    {
        Vector2 hookPositionOnScreen = Camera.main.WorldToScreenPoint(hook.transform.position);
        if (hookPositionOnScreen.y > Screen.height * 0.666f)
        {
            ;
            hookAtTop = true;
            hookAtMid = false;
            hookAtBottom = false;
            hook.transform.position = new Vector3(hook.transform.position.x,
                Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.666f,
                    hook.transform.position.z)).y, hook.transform.position.z);
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
            hook.transform.position = new Vector3(hook.transform.position.x,
                Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.333f,
                    hook.transform.position.z)).y, hook.transform.position.z);
        }
    }

    private void BackgroundEdgeRegress() //背景位置回归
    {
        if (backGround.transform.position.y < bottomPoint.y)
        {
            if (getCollection)
            {
                hookExitWater = true;
            }

            backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            backGround.transform.position = new Vector3(backGround.transform.position.x,
                bottomPoint.y, backGround.transform.position.z);
        }

        if (backGround.transform.position.y > topPoint.y)
        {
            backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            backGround.transform.position = new Vector3(backGround.transform.position.x, topPoint.y,
                backGround.transform.position.z);
        }
    }

    private void HookScroll(float hookScroll) //鱼钩滚动
    {
        Rigidbody2D rb2d = hook.GetComponent<Rigidbody2D>();
        float currentVelocity = rb2d.velocity.normalized.y;
        if (currentVelocity != lastVelocity)
        {
            variableScrollForce = fishingAbilityData.defaultScrollForce;
        }

        if (rb2d.velocity.normalized.y != 0)
        {
            if (variableScrollForce < maxScrollForce)
            {
                variableScrollForce += scrollForceGrowingSpeedIntensity;
            }
        }

        rb2d.AddForce(new Vector2(0.0f, hookScroll) * variableScrollForce, ForceMode2D.Impulse);
        rb2d.drag = drag;
        rb2d.AddForce(-rb2d.velocity.normalized * friction);
        lastVelocity = currentVelocity;
    }

    private void BackGroundScroll(float hookScroll) //背景滚动
    {
        Rigidbody2D rb2d = backGround.GetComponent<Rigidbody2D>();
        float currentVelocity = rb2d.velocity.normalized.y;
        if (currentVelocity != lastVelocity)
        {
            variableScrollForce = fishingAbilityData.defaultScrollForce;
        }

        if (rb2d.velocity.normalized.y != 0)
        {
            if (variableScrollForce < maxScrollForce)
            {
                variableScrollForce += scrollForceGrowingSpeedIntensity;
            }
        }

        rb2d.AddForce(-new Vector2(0.0f, hookScroll) * variableScrollForce, ForceMode2D.Impulse);
        rb2d.drag = drag;
        rb2d.AddForce(-rb2d.velocity.normalized * friction);
        lastVelocity = currentVelocity;
    }

    private void HorizontalMove(float hookHorizontalMove) //鱼钩水平移动
    {
        Transform transform = hook.transform;
        //transform.position = new Vector3(transform.position.x + MouseOperationDir.x * horizontalMoveIntensity, transform.position.y, transform.position.z);
        transform.Translate(hookHorizontalMove * horizontalMoveIntensity, 0.0f, 0.0f);
    }

    public void setGotCollection()
    {
        getCollection = true;
    }

    public void setHookEnterWater(FishingDataSo.BubbleAndFishData data)
    {
        Transform localCGMD = null;
        switch (data.minDepth)
        {
            case 1:
                localCGMD = collectionGenerateMinDepthArray[0];
                topPoint = topPointArray[0].position;
                bottomPoint= bottomPointArray[0].position;
                maxDepth = maxDepthArray[0];
                break;
            case 2:
                localCGMD = collectionGenerateMinDepthArray[1];
                topPoint = topPointArray[1].position;
                bottomPoint = bottomPointArray[1].position;
                maxDepth = maxDepthArray[1];
                break;
            case 3:
                localCGMD = collectionGenerateMinDepthArray[2];
                topPoint = topPointArray[2].position;
                bottomPoint = bottomPointArray[2].position;
                maxDepth = maxDepthArray[2];
                break;
            default:
                break;
        }
        GameObject collection = Instantiate(collectionPrefab, new Vector3(backGround.transform.position.x, localCGMD.position.y, backGround.transform.position.z), Quaternion.identity, backGround.transform);
        collection.GetComponent<SpriteRenderer>().sprite = data.fishSprite;
        collection.GetComponent<SpriteRenderer>().color = Color.black;
        hookEnterWater = true;
    }
    public void delayMusic1()
    {
        SoundManager.Instance.MusicPlayStr("11");
    }
}