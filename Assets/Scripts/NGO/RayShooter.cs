using System.Collections;
using UnityEngine;

public class RayShooter : FireAction
{
    [SerializeField] private Camera _playerCamera;

    private void OnValidate()
    {
        _playerCamera ??= GetComponentInChildren<Camera>();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CursorVisible(true);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reloading();
        }

        if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
        {
            CursorVisible(false);
        }
        else
        {
            CursorVisible(true);
        }
    }

    private void CursorVisible(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    protected override void Shooting()
    {
        base.Shooting();

        if (_bullets.Count > 0)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (_reloading)
        {
            yield break;
        }

        var point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
        var ray = _playerCamera.ScreenPointToRay(point);

        if (!Physics.Raycast(ray, out var hit))
        {
            yield break;
        }

        CheckHitServerRpc(ray.origin, ray.direction);
        
        var shoot = _bullets.Dequeue();
        _bulletCount = _bullets.Count.ToString();
        _ammunition.Enqueue(shoot);
        shoot.SetActive(true);
        shoot.transform.position = hit.point;
        shoot.transform.parent = hit.transform;
        yield return new WaitForSeconds(2.0f);
        shoot.SetActive(false);
    }
}