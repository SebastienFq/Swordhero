using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody m_Body;
    [SerializeField] private Health m_Health;

    [Header("Settings")]
    [SerializeField] private int m_TotalHealth = 100;
    [SerializeField] private float m_MoveSpeed = 10;

    private IEnumerator moveRoutine;
    private float m_DistanceFromPlayer = 0;

    public static Action<EnemyController> onDeath;
    public Health Health => m_Health;
    public float DistanceFromPlayer => m_DistanceFromPlayer;

    private void OnEnable()
    {
        Health.onDeath += Death;
    }

    private void OnDisable()
    {
        Health.onDeath -= Death;
    }

    public void Init()
    {
        Health.Init(m_TotalHealth);
        GoTo(FindRandomPointInMapRange(), () => { });
    }

    public void SetDistanceFromPlayer(float _Dist)
    {
        m_DistanceFromPlayer = _Dist;
    }

    private void GoTo(Vector3 _Destination, Action _Callback)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = MoveRoutine(_Destination, _Callback);
        StartCoroutine(moveRoutine);
    }

    private Vector3 FindRandomPointInMapRange()
    {
        return new Vector3(0, 0, 0);
    }

    private IEnumerator MoveRoutine(Vector3 _Destination, Action _Callback)
    {
        float timer = 0;
        Vector3 start = m_Body.position;
        Vector3 end = _Destination;
        float distance = Vector3.Distance(start, end);

        while (timer < 1)
        {
            m_Body.MovePosition(Vector3.Lerp(start, end, timer));
            timer += Time.fixedDeltaTime / (distance / m_MoveSpeed);
            yield return new WaitForFixedUpdate();
        }

        m_Body.MovePosition(end);

        _Callback();
    }

    private void Death()
    {
        onDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
