using Assets.Scripts;
using System.Collections;
using TMPro;
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
    [Header("Canvas")]
    [SerializeField] Slider blackSpawnSlider; 
    [SerializeField] Slider redSpawnSlider;
    [SerializeField] Slider whiteSpawnSlider;
    [SerializeField] Slider blueSpawnSlider;
    [Space]
    [SerializeField] TMP_Text blackCounterText;
    [SerializeField] TMP_Text redCounterText;
    [SerializeField] TMP_Text whiteCounterText;
    [SerializeField] TMP_Text blueCounterText;
    [Space]
    [SerializeField] Toggle spawnLimiterToggle;
    [SerializeField] TMP_Text spawnLimiterText;
    [SerializeField] Toggle autoSpawnToggle;

    bool spawning = true;
    float minSpawnTime = 1f;
    float maxSpawnTime = 3f;

    float blackCurrentSpawnTime = 1f;
    float redCurrentSpawnTime = 1f;
    float whiteCurrentSpawnTime = 1f;
    float blueCurrentSpawnTime = 1f;

    int blackElfsCounter;
    int redElfsCounter;
    int whiteElfsCounter;
    int blueElfsCounter;

    int maxAllowedSpawnedElfsByColor = 250;
    bool unlimitedElfSpawns = false;

    void Start()
    {
        elfsPoolingManager.Initialize(elfPreFab, initialAmountOfElfsPooled, maxAmountOfElfsPooled, this);
        explosionPoolingManager.Initialize(initialAmountOfExplosionsPooled, maxAmountOfExplosionsPooled);

        StartSpawnCoroutines();
        AddListenersToCanvas();
    }

    void Update()
    {
        blackCounterText.text = blackElfsCounter.ToString();
        redCounterText.text = redElfsCounter.ToString();
        whiteCounterText.text = whiteElfsCounter.ToString();
        blueCounterText.text = blueElfsCounter.ToString();
    }

    void StartSpawnCoroutines()
    {
        StartCoroutine(SpawnElfsByType(ElfType.Black));
        StartCoroutine(SpawnElfsByType(ElfType.Red));
        StartCoroutine(SpawnElfsByType(ElfType.White));
        StartCoroutine(SpawnElfsByType(ElfType.Blue));
    }

    void AddListenersToCanvas()
    {
        blackSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Black); });
        redSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Red); });
        whiteSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.White); });
        blueSpawnSlider.onValueChanged.AddListener(delegate { UpdateSpawnTimeFromSliderChanged(ElfType.Blue); });

        blackSpawnSlider.value = 0.5f;
        redSpawnSlider.value = 0.5f;
        whiteSpawnSlider.value = 0.5f;
        blueSpawnSlider.value = 0.5f;

        spawnLimiterToggle.onValueChanged.AddListener(delegate { unlimitedElfSpawns = !spawnLimiterToggle.isOn; });
        spawnLimiterText.text = $"Limit elf spawn to {maxAllowedSpawnedElfsByColor} per color";
        autoSpawnToggle.onValueChanged.AddListener(delegate { ChangeAutoSpawnBool(); });
    }

    public void CreateElfFromCollision(ElfController elf1, ElfController elf2, ElfType elfType)
    {
        bool shouldSpawn = false;
        Vector3 destination = new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z));

        elf1.SetNewDestination(destination);
        elf1.StartCollisionCooldown();
        elf2.SetNewDestination(destination * -1);
        elf2.StartCollisionCooldown();

        switch (elfType)
        {
            case ElfType.Black:
                shouldSpawn = blackElfsCounter < maxAllowedSpawnedElfsByColor;
                break;
            case ElfType.Red:
                shouldSpawn = redElfsCounter < maxAllowedSpawnedElfsByColor;
                break;
            case ElfType.White:
                shouldSpawn = whiteElfsCounter < maxAllowedSpawnedElfsByColor;
                break;
            case ElfType.Blue:
                shouldSpawn = blueElfsCounter < maxAllowedSpawnedElfsByColor;
                break;
            default:
                break;
        }

        if (spawning)
        {
            if (shouldSpawn || unlimitedElfSpawns)
            {
                ElfController newElf = elfsPoolingManager.Pool.Get();
                newElf.transform.position = Vector3.Lerp(elf1.transform.position, elf2.transform.position, 0.5f);
                newElf.SetNewElfToMove(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)), elfType);
                CounterAddByType(elfType);
                newElf.StartCollisionCooldown();
            }
        }
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
        bool shouldSpawn = true;

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
                    shouldSpawn = blackElfsCounter < maxAllowedSpawnedElfsByColor;
                    break;
                case ElfType.Red:
                    spawnDelay = redCurrentSpawnTime;
                    shouldSpawn = redElfsCounter < maxAllowedSpawnedElfsByColor;
                    break;
                case ElfType.White:
                    spawnDelay = whiteCurrentSpawnTime;
                    shouldSpawn = whiteElfsCounter < maxAllowedSpawnedElfsByColor;
                    break;
                case ElfType.Blue:
                    spawnDelay = blueCurrentSpawnTime;
                    shouldSpawn = blueElfsCounter < maxAllowedSpawnedElfsByColor;
                    break;
                default:
                    spawnDelay = 1f;
                    break;
            }
            yield return new WaitForSeconds(spawnDelay);
            if (shouldSpawn || unlimitedElfSpawns)
            {
                newElf = elfsPoolingManager.Pool.Get();
                newElf.transform.position = spawnPosition;
                newElf.SetNewElfToMove(new Vector3(Random.Range(whiteSpawn.position.x, blueSpawn.position.x), 1, Random.Range(redSpawn.position.z, blackSpawn.position.z)), type);
                CounterAddByType(type);
            }
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
                redElfsCounter++;
                break;
            case ElfType.White:
                whiteElfsCounter++;
                break;
            case ElfType.Blue:
                blueElfsCounter++;
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
                redElfsCounter--;
                break;
            case ElfType.White:
                whiteElfsCounter--;
                break;
            case ElfType.Blue:
                blueElfsCounter--;
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
                blackCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * System.MathF.Round(blackSpawnSlider.normalizedValue,2));
                break;
            case ElfType.Red:
                redCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * System.MathF.Round(redSpawnSlider.normalizedValue,2));
                break;
            case ElfType.White:
                whiteCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * System.MathF.Round(whiteSpawnSlider.normalizedValue,2));
                break;
            case ElfType.Blue:
                blueCurrentSpawnTime = (minSpawnTime + (maxSpawnTime - minSpawnTime) * System.MathF.Round(blueSpawnSlider.normalizedValue, 2));
                break;
            default:
                break;
        }
    }

    void ChangeAutoSpawnBool()
    {
        spawning = !spawning;

        StopAllCoroutines();

        if(spawning)
        {
            StartSpawnCoroutines();
        }
    }
}
