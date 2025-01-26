using System;
using EventCenter;
using GameController;
using UnityEngine;
using UnityEngine.UI;

namespace ShipScene
{
    public class MoneyShower : MonoBehaviour
    {
        private Text moneyText;

        private void Awake()
        {
            EventManager.Instance.AddListener<int>(EventName.MoneyChanged, OnMoneyChanged);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<int>(EventName.MoneyChanged, OnMoneyChanged);
        }

        private void Start()
        {
            moneyText = GetComponent<Text>();
            moneyText.text = GameData.Instance.Money.ToString();
        }

        private void OnMoneyChanged(int money)
        {
            moneyText.text = money.ToString();
        }
    }
}