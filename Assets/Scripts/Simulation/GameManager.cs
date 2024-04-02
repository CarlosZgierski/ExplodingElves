using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] ElfController elfPreFab;
    [SerializeField] int maxAmountOfElfsPooled;
    [SerializeField] int initialAmountOfElfsPooled;
    [Space]
    [SerializeField] int maxAmountOfExplosionsPooled;
    [SerializeField] int initialAmountOfExplosionsPooled;
    [Space]
    [SerializeField] Transform blackSpawn;
    [SerializeField] Transform redSpawn;
    [SerializeField] Transform whiteSpawn;
    [SerializeField] Transform blueSpawn;
    [Space]
    [SerializeField] ElfsPoolingManager elfsPoolingManager;
    [SerializeField] ExplosionPoolingManager explosionPoolingManager;
    [Space]
    [SerializeField] Slider blackSpawnSlider; 
    [SerializeField] Slider redSpawnSlider;
    [SerializeField] Slider whiteSpawnSlider;
    [SerializeField] Slider blueSpawnSlider;

    bool spawning = true;
    float minSpawnTime = 0.5f;
    float maxSpawnTime = 3f;

    float blackCurrentSpawnTime = 1f;
    float redCurrentSpawnTime = 1f;
    float whiteCurrentSpawnTime = 1f;
    float blueCurrentSpawnTime = 1f;

    int blackElfsCounter;
    int redElfsCounter;
    int whiteElfsCounter;
    int blueElfsCounter;

    private void Start()
    {
        elfsPoolingManager.Initialize(elfPreFab, initialAmountOfElfsPooled, maxAmountOfElfsPooled, this);
        explosionPoolingManager.Initialize(initialAmountOfExplosionsPooled, maxAmountOfExplosionsPooled);

        StartSpawnCoroutines();
        AddListenersToSliders();
    }

    void StartSpawnCoroutines()
    {
        StartCoroutine(SpawnElfsByType(ElfType.Black));
        StartCoroutine(SpawnElfsByType(ElfType.Red));
        StartCoroutine(SpawnElfsByType(ElfType.White));
        StartCoroutine(SpawnElfsByType(ElfType.Blue));
    }

    void AddListenersToSliders()
    {
        blackSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Black); });
        redSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Red); });
        whiteSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.White); });
        blueSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Blue); });

        blackSpawnSlider.value = 0.5f;
        redSpawnSlider.value = 0.5f;
        whiteSpawnSlider.value = 0.5f;
        blueSpawnSlider.value = 0.5f;
    }

    public void CreateElfFromCollision(ElfController elf1, ElfController elf2, ElfType elfType)
    {
        Vector3 destination = new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z));

        elf1.SetNewDestination(destination);
        elf1.StartCollisionCooldown();
        elf2.SetNewDestination(destination * -1);
        elf2.StartCollisionCooldown();

        ElfController newElf = elfsPoolingManager.Pool.Get();
        newElf.transform.position = Vector3.Lerp(elf1.transform.position, elf2.transform.position, 0.5f);
        newElf.SetNewElfToMove(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)), elfType);
        CounterAddByType(elfType);
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
        ParticleSystem ps = explosionPoolingManager.Pool.Get();
        ps.gameObject.transform.position = Vector3.Lerp(controller1.transform.position, controller2.transform.position, 0.5f);
        ps.Play();

        elfsPoolingManager.Pool.Release(controller1);
        CounterSubtractByType(controller1.ElfType);
        elfsPoolingManager.Pool.Release(controller2);
        CounterSubtractByType(controller2.ElfType);
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
            CounterAddByType(type);
        }
    }

    void CounterAddByType(ElfType type)
    {
        switch (type)
        {
            case ElfType.Black:
                blackElfsCounter++;
                break;
            case ElfType.Red:
                redCurrentSpawnTime++;
                break;
            case ElfType.White:
                whiteElfsCounter++;
                break;
            case ElfType.Blue:
                blueCurrentSpawnTime++;
                break;
            default:
                break;
        }
    }

    void CounterSubtractByType(ElfType type)
    {
        switch (type)
        {
            case ElfType.Black:
                blackElfsCounter--;
                break;
            case ElfType.Red:
                redCurrentSpawnTime--;
                break;
            case ElfType.White:
                whiteElfsCounter--;
                break;
            case ElfType.Blue:
                blueCurrentSpawnTime--;
                break;
            default:
                break;
        }
    }

    void UpdateSpawnTimeFromSliderChanged(ElfType type)
    {
        switch (type)
        {
            case ElfType.Black:
                blackCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * blackSpawnSlider.normalizedValue);
                break;
            case ElfType.Red:
                redCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * redSpawnSlider.normalizedValue);
                break;
            case ElfType.White:
                whiteCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * whiteSpawnSlider.normalizedValue);
                break;
            case ElfType.Blue:
                blueCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * blueSpawnSlider.normalizedValue);
                break;
            default:
                break;
        }
    }
}
