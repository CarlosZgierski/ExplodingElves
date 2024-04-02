using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ElfController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField, Tooltip("The positions in the array should be the same as the order of the ENUM")] ElfConfigurationScriptable[] elfConfigurations;
    [Header("Visual")]
    [SerializeField] MeshRenderer thisMesh;

    float newCollisionCooldown = 1.5f; //in seconds
    bool isAbleToCollide;

    public bool IsAbleToCollide => isAbleToCollide;

    ElfType elfType;
    GameManager gameManager;

    public ElfType ElfType => elfType;

    private void Update()
    {
        if (agent.remainingDistance <= 0.2f)
        {
            gameManager.ElfCompletedPath(this);
        }
    }

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        StartCollisionCooldown();
    }

    public void SetNewElfToMove(Vector3 destination, ElfType elfType)
    {
        this.elfType = elfType;

        thisMesh.material.color = elfConfigurations[(int)elfType].ElfColor;

        agent.speed = elfConfigurations[(int)elfType].ElfSpeed;
        SetNewDestination(destination);
    }

    public void SetNewDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void OnTriggerEnter(Collider other)
    {
        ElfController otherController = other.gameObject.GetComponent<ElfController>();

        if (this.transform.GetHashCode() > other.transform.GetHashCode())
        {
            if (otherController.ElfType != this.elfType)
            {
                gameManager.ReleaseElfFromCollision(this, otherController);
            }
            else
            {
                if (this.isAbleToCollide || otherController.IsAbleToCollide)
                {
                    gameManager.CreateElfFromCollision(this, otherController, this.elfType);
                }
                else
                {
                    gameManager.ChangeElfsDestinationAfterCollision(this, otherController);
                }
            }
        }
    }

    public void StartCollisionCooldown()
    {
        StartCoroutine(StartCooldownForCollisionCoroutine());
    }

    IEnumerator StartCooldownForCollisionCoroutine()
    {
        isAbleToCollide = false;
        yield return new WaitForSeconds(newCollisionCooldown);
        isAbleToCollide = true;
    }
}
