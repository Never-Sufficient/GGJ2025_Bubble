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
            shopController.TryUpgradeShip(gameObject, level);
        }

        public void TryUpgradeRod()
        {
            shopController.TryUpgradeRod(gameObject, level);
        }

        public void TryUpgradeFishingLine()
        {
            shopController.TryUpgradeFishingLine(gameObject, level);
        }
    }
}