using UnityEngine;
using UnityEngine.Pool;

public class Puff : MonoBehaviour
{
    private IObjectPool<Puff> managedPool;
    public ParticleSystem Pts;
    public float lifeTime=1;
    private void OnEnable()
    {
        Pts = GetComponent<ParticleSystem>();
        Pts.Play();
        Enable(lifeTime);
    }

    public void SetManagedPool(IObjectPool<Puff> pool)
    {
        managedPool = pool;
    }

    public void Enable(float lifeTime)
    {
        Invoke("DestroyPuff", lifeTime);
    }

    public void DestroyPuff()
    {
        managedPool.Release(this);
    }
}
