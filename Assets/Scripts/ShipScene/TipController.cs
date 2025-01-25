using DG.Tweening;
using UnityEngine;

namespace ShipScene
{
    public class TipController:MonoBehaviour
    {
        void Start()
        {
            transform.DOScale(0.9f,0.5f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}