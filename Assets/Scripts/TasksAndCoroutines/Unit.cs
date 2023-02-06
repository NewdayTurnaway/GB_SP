using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int MIN_HEALTH = 0;
    private const int MAX_HEALTH = 100;

    [SerializeField, Range(MIN_HEALTH, MAX_HEALTH)] private int _health = 50;

    [SerializeField, Min(0)] private int _healthPoint = 5;
    [SerializeField, Min(0)] private float _durationForSeconds = 3f;
    [SerializeField, Min(0)] private float _waitForSeconds = 0.5f;

    private bool _isExpired = true;

    private void Start()
    {
        ReceiveHealing();
    }

    [ContextMenu(nameof(ReceiveHealing))]
    public void ReceiveHealing()
    {
        if (!_isExpired)
        {
            return;
        }

        Debug.Log($"Start Health: {_health}");
        StartCoroutine(HealingCoroutine(_healthPoint, _durationForSeconds, _waitForSeconds));
    }

    private IEnumerator HealingCoroutine(int healthPoint, float durationForSeconds, float waitForSeconds)
    {
        _isExpired = false;

        var count = (int)Mathf.Floor(durationForSeconds / waitForSeconds);
        for (int i = 0; i < count; i++)
        {
            if (_health == MAX_HEALTH)
            {
                Debug.Log("Max Health");
                _isExpired = true;
                yield break;
            }

            if (_health < MAX_HEALTH)
            {
                _health = Mathf.Clamp(_health + healthPoint, MIN_HEALTH, MAX_HEALTH);
                Debug.Log($"Current Health: {_health}");
                yield return new WaitForSeconds(waitForSeconds);
            }
        }
        
        _isExpired = true;
    }
}
