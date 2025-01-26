using EventCenter;
using Singleton;
using UnityEngine;

namespace GameController
{
    public class GameData : SingletonBase<GameData>
    {
        private int money = 10000;
        private int shipLevel = 1;
        private int rodLevel = 1;
        private int depthCanReach = 1;

        public int Money
        {
            get => money;
            set
            {
                money = value;
                this.TriggerEvent(EventName.MoneyChanged, money);
            }
        }

        public int ShipLevel
        {
            get => shipLevel;
            set
            {
                shipLevel = value;
                this.TriggerEvent(EventName.ShipLevelChanged, shipLevel);
            }
        }

        public int RodLevel
        {
            get => rodLevel;
            set
            {
                rodLevel = value;
                this.TriggerEvent(EventName.FishingRodLevelChanged, rodLevel);
            }
        }

        public int DepthCanReach
        {
            get => depthCanReach;
            set
            {
                depthCanReach = value;
                this.TriggerEvent(EventName.FishingLineLevelChanged, depthCanReach);
            }
        }
    }
}