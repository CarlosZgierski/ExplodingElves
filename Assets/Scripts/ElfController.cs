using UnityEngine;
using UnityEngine.AI;

public class ElfController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField, Tooltip("The positions in the array should be the same as the order of the ENUM")] ElfConfigurationScriptable[] elfConfigurations;
    [Header("Visual")]
    [SerializeField] MeshRenderer thisMesh;

    ElfType elfType;
    GameManager gameManager;
    Vector3 currentDestination;

    public ElfType ElfType => elfType;

    private void Update()
    {
        if(agent.remainingDistance <= 0.2f)
        {
            gameManager.ElfCompletedPath(this);
        }
    }

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
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
        currentDestination = destination;
    }

    public void OnCollisionEnter(Collision collision)
    {
        ElfController otherController = collision.gameObject.GetComponent<ElfController>();

        if (otherController.ElfType != this.elfType)
        {
            //Explode
            //Deactivate
        }
        else
        {
            if (this.transform.GetHashCode() > collision.transform.GetHashCode())
            {
                gameManager.CreateElfFromCollision(this, otherController, this.elfType);
            }
            //Change this elf Destination
        }
    }

}
