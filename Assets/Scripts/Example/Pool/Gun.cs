using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class Gun : MonoBehaviour
{
     [Inject] private PoolingManager poolManager;
    public IPoolingObject projectile;
    public void Update()
    {
        if(Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            Fire();
        }
    }

    public void Fire()
    {
        poolManager.Get(projectile);
    }
}
