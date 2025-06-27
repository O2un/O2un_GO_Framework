using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : IPoolingObject
{
    public void Update()
    {
        transform.Translate(transform.forward * 5 * Time.deltaTime);
    }

    protected override void OnDestroyXXX()
    {
        
    }

    protected override void OnGetXXX()
    {
        transform.position = Vector3.zero;

        UniTask.Delay(1000).ContinueWith( () => gameObject.SetActive(false)).Forget();
    }

    protected override void OnReleaseXXX()
    {
        transform.position = Vector3.zero;
    }
}
