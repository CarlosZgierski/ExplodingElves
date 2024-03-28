using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [Space]
    [SerializeField] Slider blackSpawnSlider; //need to add the slider logid, should be simple
    [SerializeField] Slider redSpawnSlider;
    [SerializeField] Slider whiteSpawnSlider;
    [SerializeField] Slider clueSpawnSlider;

    bool spawning = true;
    float minSpawnTime = 0.5f;
    float maxSpawnTime = 2f;

    float blackCurrentSpawnTime = 1f;
    float redCurrentSpawnTime = 1f;
    float whiteCurrentSpawnTime = 1f;
    float blueCurrentSpawnTime = 1f;

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

    public void CreateElfFromCollision(ElfController elf1, ElfController elf2, ElfType elfType)
    {
        Vector3 destination = new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z));

        elf1.SetNewDestination(destination);
        elf1.StartCollisionCooldown();
        elf2.SetNewDestination(destination * -1);
        elf2.StartCollisionCooldown();

        ElfController newElf = elfsPoolingManager.Pool.Get();
        newElf.transform.position = Vector3.Lerp(elf1.transform.position, elf2.transform.position, 0.5f); ;
        newElf.SetNewElfToMove(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)), elfType);
        newElf.StartCollisionCooldown();
    }

    public void ChangeElfsDestinationAfterCollision(ElfController elf1, ElfController elf2)
    {
        Vector3 destination = new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z));
        elf1.SetNewDestination(destination);
        elf2.SetNewDestination(destination * -1);
    }

    public void ReleaseElfFromCollision(ElfController controller1, ElfController controller2)
    {
        elfsPoolingManager.Pool.Release(controller1);
        elfsPoolingManager.Pool.Release(controller2);
        //create all sfx and vfx here
    }

    public void ElfCompletedPath(ElfController controller) 
    {
        controller.SetNewDestination(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)));
    }

    IEnumerator SpawnElfsByType(ElfType type)
    {
        Vector3 spawnPosition;
        ElfController newElf;
        float spawnDelay;

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
            switch (type)
            {
                case ElfType.Black:
                    spawnDelay = blackCurrentSpawnTime;
                    break;
                case ElfType.Red:
                    spawnDelay = redCurrentSpawnTime;
                    break;
                case ElfType.White:
                    spawnDelay = whiteCurrentSpawnTime;
                    break;
                case ElfType.Blue:
                    spawnDelay = blueCurrentSpawnTime;
                    break;
                default:
                    spawnDelay = 1f;
                    break;
            }
            yield return new WaitForSeconds(spawnDelay);
            newElf = elfsPoolingManager.Pool.Get();
            newElf.transform.position = spawnPosition;
            newElf.SetNewElfToMove(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)), type);
        }
    }
}
