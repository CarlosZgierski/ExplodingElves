using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts
{
    public class ExplosionPoolingManager : MonoBehaviour
    {
        [SerializeField] GameObject explosionPrefab;
        [SerializeField] Transform explosionTransformParent;
        
        IObjectPool<ParticleSystem> pool;

        public IObjectPool<ParticleSystem> Pool => pool;

        public void Initialize(int initialPoolSize, int maxPoolSize)
        {
            if (pool == null)
            {
                pool = new ObjectPool<ParticleSystem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, initialPoolSize, maxPoolSize);
            }
        }

        ParticleSystem CreatePooledItem()
        {
            GameObject ps = Instantiate(explosionPrefab, explosionTransformParent);
            ps.GetComponent<ReturnToPool>().Initialize(this);

            return ps.GetComponent<ParticleSystem>();
        }

        void OnReturnedToPool(ParticleSystem ps)
        {
            ps.gameObject.SetActive(false);
        }

        void OnTakeFromPool(ParticleSystem ps)
        {
            ps.gameObject.SetActive(true);
        }

        void OnDestroyPoolObject(ParticleSystem ps)
        {
            Destroy(ps.gameObject);
        }
    }
}