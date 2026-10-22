using UnityEngine;
using UnityEngine.Pool;

namespace Project.Source
{
    public class CoinsFactory
    {
        public ObjectPool<CoinController> Pool { get; }

        public CoinsFactory()
        {
            var prefab = Resources.Load<CoinController>("Coin");
            Pool = new ObjectPool<CoinController>(() =>
                {
                    var obj = Object.Instantiate(prefab);
                    obj.Pool = Pool;
                    return obj;
                },
                c =>
                {
                    c.gameObject.SetActive(true);
                    c.Init();
                }, c => c.gameObject.SetActive(false),
                c => c.Pool = null);
        }
    }
}