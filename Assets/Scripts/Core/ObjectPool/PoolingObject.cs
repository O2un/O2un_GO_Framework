using O2un.Core;
using UnityEngine;
using VContainer;

public abstract class IPoolingObject : SafeMonoBehaviour
{
    [Inject] protected PoolingManager poolingManager;

    protected override void Init()
    {
        gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        poolingManager?.Return(this);
    }

    public void OnGet()
    {
        OnGetXXX();

        gameObject.SetActive(true);
    }
    protected abstract void OnGetXXX();
    public void OnDestroy()
    {
        OnDestroyXXX();
    }
    protected abstract void OnDestroyXXX();
    public void OnRelease()
    {
        OnReleaseXXX();
        
        gameObject.SetActive(false);
    }
    protected abstract void OnReleaseXXX();
}
