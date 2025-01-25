using Singleton;

namespace GameController
{
    public class GameData : SingletonBase<GameData>
    {
        public float Money = 0;
        public int ShipLevel = 0;
    }
}