using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : IPoolingObject
{
    public void Update()
    {
        transform.Translate(Vector3.forward * 5 * Time.deltaTime);
    }

    protected override void OnDestroyXXX()
    {
        
    }

    protected override void OnGetXXX()
    {
        transform.position = Vector3.zero;
        DelayCall(1, () => gameObject.SetActive(false));
    }

    protected override void OnReleaseXXX()
    {
        transform.position = Vector3.zero;
    }
}
