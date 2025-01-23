namespace Singleton
{
    public class SingletonBase<T> where T:new()
    {
        private static T _instance;
        // 多线程安全机制
        private static readonly object Locker = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    //lock写第一个if里是因为只有该类的实例还没创建时，才需要加锁，这样可以节省性能
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }
}