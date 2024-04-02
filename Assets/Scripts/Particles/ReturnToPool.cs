using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;

    ExplosionPoolingManager poolingManager;

    public void Initialize(ExplosionPoolingManager poolingManager)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        this.poolingManager = poolingManager;
    }

    private void OnEnable()
    {
        particleSystem.Play();
        StartCoroutine(StopParticleSystem());
    }

    IEnumerator StopParticleSystem ()
    {
        yield return new WaitForSeconds(1.5f);
        particleSystem.Stop();
        particleSystem.Clear();
    }

    void OnParticleSystemStopped()
    {
        poolingManager.Pool.Release(particleSystem);
    }
}
