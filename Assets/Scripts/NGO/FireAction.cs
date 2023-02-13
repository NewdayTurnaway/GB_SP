using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public abstract class FireAction : NetworkBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _startAmmunition = 20;
    [SerializeField, Min(0)] protected int _damage = 25;

    protected string _bulletCount = string.Empty;
    protected Queue<GameObject> _bullets = new();
    protected Queue<GameObject> _ammunition = new();
    protected bool _reloading = false;

    public string BulletCount => _bulletCount;

    protected virtual void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        for (var i = 0; i < _startAmmunition; i++)
        {
            GameObject bullet;

            if (_bulletPrefab == null)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                bullet = Instantiate(_bulletPrefab);
            }

            bullet.SetActive(false);
            _ammunition.Enqueue(bullet);
        }
    }

    public virtual async void Reloading()
    {
        _bullets = await Reload();
    }

    protected virtual void Shooting()
    {
        if (_bullets.Count == 0)
        {
            Reloading();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckHitServerRpc(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out var hit))
        {
            if (hit.transform.TryGetComponent(out PlayerNetwork player))
            {
                if (player.OwnerClientId == OwnerClientId)
                {
                    return;
                }

                player.TakeDamageClientRpc(_damage);
            }
        }
    }

    private async Task<Queue<GameObject>> Reload()
    {
        if (!_reloading)
        {
            _reloading = true;
            StartCoroutine(ReloadingAnim());
            return await Task.Run(delegate
            {
                var cage = 10;

                if (_bullets.Count < cage)
                {
                    Thread.Sleep(3000);
                    var bullets = _bullets;

                    while (bullets.Count > 0)
                    {
                        _ammunition.Enqueue(bullets.Dequeue());
                    }

                    cage = Mathf.Min(cage, _ammunition.Count);

                    if (cage > 0)
                    {
                        for (var i = 0; i < cage; i++)
                        {
                            var sphere = _ammunition.Dequeue();
                            bullets.Enqueue(sphere);
                        }
                    }
                }

                _reloading = false;
                return _bullets;
            });
        }
        else
        {
            return _bullets;
        }
    }

    private IEnumerator ReloadingAnim()
    {
        while (_reloading)
        {
            _bulletCount = " | ";
            yield return new WaitForSeconds(0.01f);
            _bulletCount = @" \ ";
            yield return new WaitForSeconds(0.01f);
            _bulletCount = "---";
            yield return new WaitForSeconds(0.01f);
            _bulletCount = " / ";
            yield return new WaitForSeconds(0.01f);
        }

        _bulletCount = _bullets.Count.ToString();
        yield return null;
    }
}