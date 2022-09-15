using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Texture m_BaseTexture;
    [SerializeField] private Texture m_AngryTexture;
    [SerializeField] private Transform m_Graphics;
    [SerializeField] private Rigidbody m_Body;
    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private Health m_Health;
    [SerializeField] private Transform m_ModelCenter;
    [SerializeField] private ParticleSystem m_FireParticles;
    [SerializeField] private ParticleSystem m_HitParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private int m_TotalHealth = 100;
    [SerializeField] private int m_Damages = 24;
    [SerializeField] private float m_WanderingMoveSpeed = 2;
    [SerializeField] private float m_AttackingMoveSpeed = 3.5f;
    [SerializeField] private float m_MinMoveDistance = 2;
    [SerializeField] private float m_OffsetMoveDistance = 4;
    [SerializeField] private float m_AttackDuration = 8;
    [SerializeField] private float m_AttackDurationOffset = 4;
    [SerializeField] private float m_WanderDuration = 8;
    [SerializeField] private float m_WanderDurationOffset = 4;
    [SerializeField] private float m_WanderPauseDuration = 2;
    [SerializeField] private float m_WanderPauseDurationOffset = 2;
    [SerializeField] private float m_DelayBeforeDangerous = 1;


    //private IEnumerator moveRoutine;
    private float m_DistanceFromPlayer = 0;
    private bool m_CanDamage = false;
    private bool m_IsAttacking = false;
    private bool m_IsWandering = false;
    private PlayerController m_Player;
    private Vector3 m_Destination;
    private float m_TimerAction;
    private float m_TimerPause;
    private float m_BaseTimerAction;
    private IEnumerator m_RescaleGraphicsRoutine;
    private bool m_IsDead;

    public static Action<EnemyController> onDeath;
    public Health Health => m_Health;
    public float DistanceFromPlayer => m_DistanceFromPlayer;
    public bool CanDamage => m_CanDamage;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;

        Health.onDeath += Death;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;

        Health.onDeath -= Death;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_IsDead)
            return;

        if(other.gameObject.tag == Constants.Tags.c_WeaponHitbox)
        {
            var hitMarker = Instantiate(UnitManager.Instance.HitMarkerPrefab, transform.position, Quaternion.identity);
            var hitBox = other.GetComponent<HitBox>();
            var isCritical = Random.value <= hitBox.CritChance;
            var damages = hitBox.Damages * (isCritical ? 2 : 1);
            hitMarker.Init(transform, damages, isCritical);
            m_Health.AddHealth(-damages);
            m_Animator.SetTrigger(Constants.AnimatorValues.c_Hit);
            var particles = Instantiate(m_HitParticlesPrefab, m_ModelCenter.position, Quaternion.identity);
            particles.transform.localScale = (transform.localScale);
        }
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        if (m_IsDead)
            return;

        if (m_IsAttacking)
        {
            m_Agent.SetDestination(m_Player.transform.position);

            m_TimerAction -= Time.deltaTime;

            m_CanDamage = m_BaseTimerAction - m_TimerAction > m_DelayBeforeDangerous;

            if (m_TimerAction <= 0)
            {
                StopAttack();
            }
        }
        else
        {
            if(m_IsWandering)
            {
                m_Agent.SetDestination(m_Destination);

                if (Vector3.Distance(transform.position, m_Agent.destination) < 0.1f)
                {
                    ReachDestination();
                }
            }
            else
            {
                m_TimerPause -= Time.deltaTime;

                if (m_TimerPause <= 0)
                {                 
                    if(FindRandomDestination())
                        m_IsWandering = true;
                }
            }

            m_TimerAction -= Time.deltaTime;

            if(m_TimerAction <= 0)
            {
                StartAttack();
            }
        }

        m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, m_Agent.velocity.magnitude > 0.25f);
    }

    public void Init()
    {
        m_Health.Init(m_TotalHealth + GameManager.Instance.PlayerLevel * 10);
        m_Player = UnitManager.Instance.Player;
        StopAttack();
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.RESET:
                break;
        }
    }

    public int GetDamages()
    {
        return m_Damages + GameManager.Instance.PlayerLevel;
    }

    private bool FindRandomDestination()
    {
        Vector2 c = Random.insideUnitCircle;
        var randomDirection = new Vector3(c.x, 0, c.y);

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(transform.position + randomDirection * (m_MinMoveDistance + m_OffsetMoveDistance * UnityEngine.Random.value), out hit, m_MinMoveDistance + m_OffsetMoveDistance, NavMesh.AllAreas))
        {
            return false;
        }

        m_Destination = hit.position;
        ExtensionMethods.DrawDot(m_Destination, 1, Color.magenta, 5f);
        return true;
    }

    private void StartAttack()
    {
        m_IsAttacking = true;
        m_IsWandering = false;
        m_Agent.speed = m_AttackingMoveSpeed;
        m_BaseTimerAction = m_TimerAction = m_AttackDuration + m_AttackDurationOffset * Random.value;
        m_Renderer.material.SetColor(Constants.GameplayValues.c_EmissiveColor, Color.red);
        m_Renderer.material.SetTexture("_BaseMap", m_AngryTexture);
        RescaleGraphics(1.8f, 0.5f);
        m_FireParticles.Play();
    }

    private void StopAttack()
    {
        m_IsAttacking = false;
        m_IsWandering = false;
        m_CanDamage = false;
        m_Agent.speed = m_WanderingMoveSpeed;
        m_BaseTimerAction = m_TimerAction = m_WanderDuration + m_WanderDurationOffset * Random.value;
        m_Renderer.material.SetColor(Constants.GameplayValues.c_EmissiveColor, Color.black);
        m_Renderer.material.SetTexture("_BaseMap", m_BaseTexture);
        RescaleGraphics(1f, 0.5f);
        m_FireParticles.Stop();
    }

    private void ReachDestination()
    {
        m_TimerPause = m_WanderPauseDuration + m_WanderPauseDurationOffset * Random.value;
        m_IsWandering = false;       
    }

    private void RescaleGraphics(float _Target, float _Duration, float _Delay = 0)
    {
        if (m_RescaleGraphicsRoutine != null) StopCoroutine(m_RescaleGraphicsRoutine);
        m_RescaleGraphicsRoutine = RescaleGraphicsRoutine(_Target, _Duration, _Delay);
        StartCoroutine(m_RescaleGraphicsRoutine);
    }
    
    private IEnumerator RescaleGraphicsRoutine(float _Target, float _Duration, float _Delay = 0)
    {
        if (_Delay > 0)
            yield return new WaitForSeconds(_Delay);

        float timer = 0;
        Vector3 start = m_Graphics.localScale;
        Vector3 end = Vector3.one * _Target;
        
        while(timer < 1)
        {
            m_Graphics.localScale = Vector3.Lerp(start, end, CalculationEasing.EaseOutSine(0, 1, timer));
            timer += Time.deltaTime / _Duration;
            yield return new WaitForEndOfFrame();
        }

        m_Graphics.localScale = end;
    }

    public void SetDistanceFromPlayer(float _Dist)
    {
        m_DistanceFromPlayer = _Dist;
    }

    public void Destroy()
    {
        m_Health.Destroy();
        Destroy(gameObject);
    }

    private void Death()
    {
        onDeath?.Invoke(this);
        StopAttack();
        m_IsDead = true;
        m_Agent.SetDestination(transform.position);
        m_Animator.SetTrigger(Constants.AnimatorValues.c_Death);
        RescaleGraphics(0, 0.2f, 1.2f);
        Destroy(gameObject, 2);
    }
}
