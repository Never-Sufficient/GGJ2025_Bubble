using Cinemachine;
using UnityEngine;

namespace ShipScene
{
    public class CheckboxController : MonoBehaviour
    {
        [SerializeField] private ShopController shopController;
        [SerializeField] private int level;

        public void TryUpgradeShip()
        {
            SoundManager.Instance.EffectPlayStr("2");
            shopController.TryUpgradeShip(gameObject, level);
        }

        public void TryUpgradeRod()
        {
            SoundManager.Instance.EffectPlayStr("2");
            shopController.TryUpgradeRod(gameObject, level);
        }

        public void TryUpgradeFishingLine()
        {
            SoundManager.Instance.EffectPlayStr("2");
            shopController.TryUpgradeFishingLine(gameObject, level);
        }
    }
}