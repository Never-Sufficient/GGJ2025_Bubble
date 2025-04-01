using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using EventCenter;
using System.Collections;
using System.Collections.Generic;
using cfg;
using GameController;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using YooAsset;

public class FishingRodInputHandle : MonoBehaviour
{
    [SerializeField] private GameObject BubbleGeneratorPrefab;
    [SerializeField] private GameObject collectionPrefab;
    [SerializeField] private GameObject hook; //底部鱼钩
    [SerializeField] private GameObject backGround; //可移动背景
    [SerializeField] private GameObject trapPrefab;
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
    [SerializeField] private Transform[] rocksGenerateTransforms;
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
    private Vector3 lastCursorPosition = Vector3.zero;
    private bool rocksCompleted = false;
    private void Awake()
    {
        EventManager.Instance.AddListener<FishCfg>(EventName.StartFishing, setHookEnterWater);
        EventManager.Instance.AddListener(EventName.TimerExpire, OnTimeExpire);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<FishCfg>(EventName.StartFishing, setHookEnterWater);
        EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTimeExpire);
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
        if (isFishing)
        {
            accumulatedScroll += hookScrollAction.ReadValue<float>();
            accumulatedMove += hookHorizontalMoveAction.ReadValue<float>();
        }
    }

    private async void FixedUpdate()
    {
        //Debug.Log(Mathf.Abs(hook.GetComponent<Rigidbody2D>().velocity.y));
        // Debug.Log("getCollection" + getCollection);
        // Debug.Log("hookExitWater" + hookExitWater);
        // Debug.Log(backGround.transform.position.y > topPoint.transform.position.y);

        if (hookEnterWater)
        {
            HookEnterWater();
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
                HookScroll(hookScroll).Forget();
            }

            if (hookAtBottom && hookScroll > 0)
            {
                backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                HookScroll(hookScroll).Forget();
            }

            if (hookAtTop && hookScroll < 0)
            {
                backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                HookScroll(hookScroll).Forget();
            }

            if ((hookAtTop && hookScroll > 0) || (hookAtBottom && hookScroll < 0))
            {
                if (backGround.transform.position.y <= topPoint.y &&
                    backGround.transform.position.y >= bottomPoint.y)
                {
                    BackGroundScroll(hookScroll).Forget();
                    //currentDepth = Mathf.Abs(bottomPoint.position.y - hook.transform.position.y) / Mathf.Abs(topPoint.position.y - bottomPoint.transform.position.y) * maxDepth;
                }
            }

            HorizontalMove(hookHorizontalMove);
            GetHookPositionOnScreen();
            BackgroundEdgeRegress();
        }
    }

    private  void HookEnterWater()
    {
        Invoke("delayMusic1", 0.5f);
        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(enterWaterPosition.position.x,
                Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.666f,
                    hook.transform.position.z)).y), Time.deltaTime * 2.0f);
        if (Vector2.Distance(
                new Vector2(enterWaterPosition.position.x,
                    Camera.main.ScreenToWorldPoint(new Vector3(hook.transform.position.x, Screen.height * 0.666f,
                        hook.transform.position.z)).y), transform.position) < 0.01f)
        {
            transform.position = new Vector2(transform.position.x, enterWaterPosition.position.y);
            hookEnterWater = false;
            isFishing = true;
        }
    }

    private void HookExitWater()
    {
        Invoke("delayMusic2", 0.5f);
        isFishing = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = Vector2.MoveTowards(transform.position, exitWaterPosition.position, Time.deltaTime * 2.0f);
        if (Vector2.Distance(exitWaterPosition.position, transform.position) < 0.01f)
        {
            hook.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.position = new Vector2(transform.position.x, exitWaterPosition.position.y);
            hookExitWater = false;
            getCollection = false;
            rocksCompleted = false;
            this.TriggerEvent(EventName.CaughtFish);
            Mouse.current.WarpCursorPosition(lastCursorPosition);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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

    private async UniTaskVoid HookScroll(float hookScroll) //鱼钩滚动
    {
        await UniTask.WaitForFixedUpdate();
        Rigidbody2D rb2d = hook.GetComponent<Rigidbody2D>();
        float currentVelocity = rb2d.velocity.normalized.y;
        if (currentVelocity * lastVelocity <= 0)
        {
            variableScrollForce = fishingAbilityData.defaultScrollForce;
        }

        if (rb2d.velocity.normalized.y != 0)
        {
            if (variableScrollForce < maxScrollForce)
            {
                variableScrollForce += scrollForceGrowingSpeedIntensity;
            }
            if (!SoundManager.Instance.getHookState() && Mathf.Abs( rb2d.velocity.y) >= 1f)
            {
                SoundManager.Instance.PlayHookSound();
            }

        }
        if(Mathf.Abs(rb2d.velocity.y) < 1f)
        {
            //print(1);
            SoundManager.Instance.StopHookSound();
        }

        rb2d.AddForce(new Vector2(0.0f, hookScroll) * variableScrollForce, ForceMode2D.Impulse);
        rb2d.drag = drag;
        rb2d.AddForce(-rb2d.velocity.normalized * friction);
        lastVelocity = currentVelocity;
    }

    private async UniTaskVoid BackGroundScroll(float hookScroll) //背景滚动
    {
        await UniTask.WaitForFixedUpdate();
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
            if (!SoundManager.Instance.getHookState() && Mathf.Abs(rb2d.velocity.y) >= 1f)
            {
                SoundManager.Instance.PlayHookSound();
            }
        }
        if (Mathf.Abs(rb2d.velocity.y) < 1f)
        {
           // print(2);
            SoundManager.Instance.StopHookSound();
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

    public void setGotCollection(bool getCollection)
    {
        this.getCollection = getCollection;
    }

    public async void setHookEnterWater(FishCfg data)
    {
        Cursor.visible = false;
        lastCursorPosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.Confined;
        Transform localCGMD = null;
        maxDepth = maxDepthArray[GameData.Instance.DepthCanReach - 1];
        switch (GameData.Instance.DepthCanReach)
        {
            case 1:
                localCGMD = collectionGenerateMinDepthArray[0];
                topPoint = topPointArray[0].position;
                bottomPoint = bottomPointArray[0].position;

                break;
            case 2:
                localCGMD = collectionGenerateMinDepthArray[1];
                topPoint = topPointArray[1].position;
                bottomPoint = bottomPointArray[1].position;
                break;
            case 3:
                localCGMD = collectionGenerateMinDepthArray[2];
                topPoint = topPointArray[2].position;
                bottomPoint = bottomPointArray[2].position;
                break;
            default:
                break;
        }
        Instantiate(trapPrefab, new Vector3(backGround.transform.position.x + (Random.value > 0.5f ? 2.8f : -2.8f), collectionGenerateMinDepthArray[0].position.y +(Random.Range(3.75f, 4.75f)* (Random.value > 0.5f ? 1 : -1)), backGround.transform.position.z -1),
            Quaternion.identity, backGround.transform);
        Instantiate(trapPrefab, new Vector3(backGround.transform.position.x + (Random.value > 0.5f ? 2.8f : -2.8f), collectionGenerateMinDepthArray[1].position.y + (Random.Range(3.75f, 4.75f) * (Random.value > 0.5f ? 1 : -1)), backGround.transform.position.z - 1),
            Quaternion.identity, backGround.transform);
        for(int i = 0;i< rocksGenerateTransforms.Length; i++)
        {
            if(Random.value > 0.333f && !rocksCompleted)
            {
                Instantiate(trapPrefab, new Vector3(backGround.transform.position.x + (Random.value > 0.5f ? 2.8f : -2.8f), rocksGenerateTransforms[i].position.y + (Random.Range(0f, 0.5f) * (Random.value > 0.5f ? 1 : -1)), backGround.transform.position.z - 1),
            Quaternion.identity, backGround.transform);
            }
        }
        rocksCompleted = true;
        GameObject collection = Instantiate(collectionPrefab,
            new Vector3(backGround.transform.position.x, localCGMD.position.y, backGround.transform.position.z),
            Quaternion.identity, backGround.transform);
        var pathPrefix = "Assets/Arts/Sprites/Fish/";
        var package=YooAssets.GetPackage("DefaultPackage");
        // collection.GetComponent<SpriteRenderer>().sprite = package.LoadSubAssetsSync(pathPrefix + data.SpritePath).GetSubAssetObject<Sprite>(data.SpritePath);
        var handle = package.LoadSubAssetsAsync(pathPrefix + data.SpritePath);
        await handle.ToUniTask();
        collection.GetComponent<SpriteRenderer>().sprite = handle.GetSubAssetObject<Sprite>(data.SpritePath);
        Instantiate(BubbleGeneratorPrefab,
            new Vector3(backGround.transform.position.x, localCGMD.position.y, backGround.transform.position.z),
            Quaternion.Euler(new Vector3(-90f, 0f, 0f)), backGround.transform);
        hookEnterWater = true;
    }
    private async UniTaskVoid NoTimeToGetCollection(Vector2 position)
    {
        if (isFishing)
        {
            isFishing = false;
            hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            getCollection = false;
            hookExitWater = false;
            rocksCompleted = false;
            Mouse.current.WarpCursorPosition(lastCursorPosition);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            await UniTask.WaitForFixedUpdate();
            await backGround.GetComponent<Rigidbody2D>().DOMove(bottomPoint, 2.5f);
            gameObject.GetComponent<Rigidbody2D>().DOMove(position, 3f).SetEase(Ease.InOutCubic).onComplete = () =>
            {
                GetComponent<Collider2D>().enabled = true;

            };
        }
        Invoke("delayMusic2", 1.5f);
    }
    private void OnTimeExpire()
    {
        GetComponent<Collider2D>().enabled = false;
        NoTimeToGetCollection(exitWaterPosition.position).Forget();
    }
    public void delayMusic1()
    {
        SoundManager.Instance.MusicPlayStr("11");
    }
    public async UniTaskVoid delayMusic2()
    {
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.StopHookSound();
        if(isFishing)
        {
            isFishing = false;
            hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            backGround.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            getCollection = false;
            hookExitWater = false;
            rocksCompleted = false;
            Mouse.current.WarpCursorPosition(lastCursorPosition);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            await UniTask.WaitForFixedUpdate();
            await backGround.GetComponent<Rigidbody2D>().DOMove(bottomPoint, 1f);
            gameObject.GetComponent<Rigidbody2D>().DOMove(exitWaterPosition.position, 1.5f).SetEase(Ease.InOutCubic).onComplete = () =>
            {
                GetComponent<Collider2D>().enabled = true;

            };
        }

    }
}