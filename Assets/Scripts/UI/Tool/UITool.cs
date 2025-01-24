using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// UI工具类
/// </summary>
public class UITool : MonoBehaviour
{
    private static UITool _instance;
    public static UITool Instance
    {
        get
        {
            if (_instance == null)
            {

            }
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    // 缩放UI 
    public Coroutine ScaleUI(RectTransform uiElement, Vector3 targetScale, float duration)
    {
        return StartCoroutine(ScaleCoroutine(uiElement, targetScale, duration, null));
    }
    // 缩放UI 带回调
    public Coroutine ScaleUI(RectTransform uiElement, Vector3 targetScale, float duration, Action onComplete)
    {
        return StartCoroutine(ScaleCoroutine(uiElement, targetScale, duration, onComplete));
    }

    private IEnumerator ScaleCoroutine(RectTransform uiElement, Vector3 targetScale, float duration, Action onComplete)
    {
        Vector3 startScale = uiElement.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            uiElement.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiElement.localScale = targetScale;
        onComplete?.Invoke();
    }

    // MoveUI 传入速度曲线
    public void MoveUI(RectTransform uiElement, Vector2 direction, AnimationCurve speedCurve, float distance)
    {
        StartCoroutine(MoveWithCurveCoroutine(uiElement, direction, speedCurve, distance, null));
    }
    // MoveUI 传入速度
    public void MoveUI(RectTransform uiElement, Vector2 direction, float speed, float distance)
    {
        StartCoroutine(MoveWithSpeedCoroutine(uiElement, direction, speed, distance, null));
    }
    // MoveUI 传入速度曲线带回调
    public Coroutine MoveUI(RectTransform uiElement, Vector2 direction, AnimationCurve speedCurve, float distance, Action onComplete)
    {
        return StartCoroutine(MoveWithCurveCoroutine(uiElement, direction, speedCurve, distance, onComplete));
    }
    // MoveUI 传入速度曲线带回调
    public Coroutine MoveUI(RectTransform uiElement, Vector2 direction, float speed, float distance, Action onComplete)
    {
        return StartCoroutine(MoveWithSpeedCoroutine(uiElement, direction, speed, distance, onComplete));
    }

    private IEnumerator MoveWithCurveCoroutine(RectTransform uiElement, Vector2 direction, AnimationCurve speedCurve, float distance, Action onComplete)
    {
        Vector2 startPosition = uiElement.anchoredPosition;
        Vector2 targetPosition = startPosition + direction.normalized * distance;
        float elapsedTime = 0f;
        float duration = speedCurve.keys[speedCurve.length - 1].time;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float speed = speedCurve.Evaluate(t);
            uiElement.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition;
        onComplete?.Invoke();
    }

    private IEnumerator MoveWithSpeedCoroutine(RectTransform uiElement, Vector2 direction, float speed, float distance, Action onComplete)
    {
        Vector2 startPosition = uiElement.anchoredPosition;
        Vector2 targetPosition = startPosition + direction.normalized * distance;
        float totalDistance = Vector2.Distance(startPosition, targetPosition);
        float duration = totalDistance / speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            uiElement.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition;
        onComplete?.Invoke();
    }


}
