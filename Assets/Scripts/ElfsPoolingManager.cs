using UnityEngine;
using UnityEngine.Pool;

public class ElfsPoolingManager : MonoBehaviour
{
    IObjectPool<ElfController> pool;
    ElfController prefab;
    GameManager gameManager;

    public IObjectPool<ElfController> Pool => pool;

    public void Initialize(ElfController elfControllerPrefab, int initialPoolSize, int maxPoolSize, GameManager gameManager)
    {
        if (pool == null)
        {
            pool = new ObjectPool<ElfController>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, initialPoolSize, maxPoolSize);
            this.gameManager = gameManager;
            prefab = elfControllerPrefab;
        }
    }

    ElfController CreatePooledItem()
    {
        ElfController elfController = Instantiate(prefab);
        elfController.Initialize(gameManager);
        return elfController;
    }

    void OnReturnedToPool(ElfController controller)
    {
        controller.gameObject.SetActive(false);
    }

    void OnTakeFromPool(ElfController controller)
    {
        controller.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(ElfController controller)
    {
        Destroy(gameObject.gameObject);
    }
}
