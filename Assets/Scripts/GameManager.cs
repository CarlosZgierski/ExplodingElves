using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] ElfController elfPreFab;
    [SerializeField] int maxAmountOfElfsPooled;
    [SerializeField] int initialAmountOfElfsPooled;
    [Space]
    [SerializeField] Transform blackSpawn;
    [SerializeField] Transform redSpawn;
    [SerializeField] Transform whiteSpawn;
    [SerializeField] Transform blueSpawn;
    [Space]
    [SerializeField] ElfsPoolingManager elfsPoolingManager;

    bool spawning = true;
    float minSpawnTime = 0.5f;
    float maxSpawnTime = .5f;

    private void Start()
    {
        elfsPoolingManager.Initialize(elfPreFab, initialAmountOfElfsPooled, maxAmountOfElfsPooled, this);

        StartSpawnCoroutines();
    }

    void StartSpawnCoroutines()
    {
        StartCoroutine(SpawnElfsByType(ElfType.Black));
        StartCoroutine(SpawnElfsByType(ElfType.Red));
        StartCoroutine(SpawnElfsByType(ElfType.White));
        StartCoroutine(SpawnElfsByType(ElfType.Blue));
    }

    public void CreateElfFromCollision(ElfController controller1, ElfController controller2, ElfType elfType)
    {
        //controller1.SetNewDestination();
        //controller2.SetNewDestination();

        //Create new elf here
    }

    public void ReleaseElfFromCollision(ElfController controller1, ElfController controller2)
    {

    }

    public void ElfCompletedPath(ElfController controller) 
    {
        elfsPoolingManager.Pool.Release(controller);
    }

    IEnumerator SpawnElfsByType(ElfType type)
    {
        Vector3 spawnPosition;
        switch (type)
        {
            case ElfType.Black:
                spawnPosition = blackSpawn.position;
                break;
            case ElfType.Red:
                spawnPosition = redSpawn.position;
                break;
            case ElfType.White:
                spawnPosition = whiteSpawn.position;
                break;
            case ElfType.Blue:
                spawnPosition = blueSpawn.position;
                break;
            default:
                spawnPosition = Vector3.zero;
                break;
        }

        while (spawning)
        {
            yield return new WaitForSecondsRealtime(maxSpawnTime);
            ElfController newElf = elfsPoolingManager.Pool.Get();
            newElf.transform.position = spawnPosition;
            newElf.SetNewElfToMove(new Vector3(Random.Range(-125f,125f), 1, Random.Range(-125f, 125f)), type);
        }
    }
}
