using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("测试动画方向")]
    public Vector2 TestMoveDir;
    [SerializeField]
    [Tooltip("测试动画曲线")]
    public AnimationCurve TestMoveCur;
    [SerializeField]
    [Tooltip("测试动画速度")]
    public float TestMoveSpeed;
    [SerializeField]
    [Tooltip("测试动画距离")]
    public float TestMoveDistance;
    [SerializeField]
    [Tooltip("测试缩放时间")]
    public float TestScaleTime;
    [SerializeField]
    [Tooltip("测试缩放大小")]
    public Vector3 TestScale;
    RectTransform rectTransform;
    Coroutine coroutine;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();


    }
    public void TestMove()
    {
        coroutine = UITool.Instance.MoveUI(rectTransform, TestMoveDir, TestMoveCur, TestMoveDistance, TestMove);
    }

    private void OnGUI()
    {

        if (GUILayout.Button("Move"))
        {
            TestMove();
        }
        if (GUILayout.Button("Big"))
        {
            UITool.Instance.ScaleUI(rectTransform, TestScale, TestScaleTime);
        }
        if (GUILayout.Button("Stop"))
        {
            StopCoroutine(coroutine);
        }

    }


}
