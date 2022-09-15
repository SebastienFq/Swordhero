using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private const float c_MoveSpeedAnimation = 3f;

    [Header("References")]
    [SerializeField] private Transform m_Graphics = null;
    [SerializeField] private Collider m_SelfCollider = null;
    [SerializeField] private Joystick m_Joystick = null;
    [SerializeField] private Animator m_Animator = null;
    [SerializeField] private PlayerAnimatorListener m_AnimatorListener = null;
    [SerializeField] private Transform m_WeaponSlot = null;
    [SerializeField] private Renderer m_PlayerRenderer = null;
    [SerializeField] private GameObject m_TargetIndicator = null;
    [SerializeField] private HitBox m_Hitbox = null;
    [SerializeField] private Health m_Health = null;

    [Header("Settings")]
    [SerializeField] private int m_MaxHealth = 100;
    [SerializeField] private float m_MoveSpeed = 5;

    private NavMeshHit m_NavMeshHit;
    private EnemyController m_NearestUnit;
    private EnemyController m_Target;
    private Weapon m_EquippedWeapon;

    private bool m_HasWeaponEquipped;
    private bool m_IsMoving;
    private bool m_HasTarget;
    private bool m_HasNearestUnit;
    private bool m_LockMovements = false;

    private Vector2 m_JoyDir;
    private Vector3 m_WorldDir;

    private float m_MovementSpeedMultiplier = 1;
    private float m_AttackSpeedMultiplier = 1;

    private int m_WeaponType;

    public Health Health => m_Health;
    public Weapon EquippedWeapon => m_EquippedWeapon;

    public static Action<ItemData> onWeaponEquipped;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded += OnGamePhaseEnded;
        LevelManager.onLevelSpawned += OnLevelSpawned;
        UnitManager.onUnitsUpdated += OnUnitsUpdated;
        EnemyController.onDeath += OnEnemyDeath;

        m_AnimatorListener.onDamage += ActivateHitBox;
        m_AnimatorListener.onDamage += ActivateWeaponParticles;
        m_AnimatorListener.onEndAttack += OnEndAttack;
        m_AnimatorListener.onHit += OnHit;

        m_Health.onDeath += OnDeath;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded -= OnGamePhaseEnded;
        LevelManager.onLevelSpawned -= OnLevelSpawned;
        UnitManager.onUnitsUpdated -= OnUnitsUpdated;
        EnemyController.onDeath -= OnEnemyDeath;

        m_AnimatorListener.onDamage -= ActivateHitBox;
        m_AnimatorListener.onDamage -= ActivateWeaponParticles;
        m_AnimatorListener.onEndAttack -= OnEndAttack;
        m_AnimatorListener.onHit -= OnHit;

        m_Health.onDeath -= OnDeath;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tags.c_LootWeapon))
        {
            var loot = other.GetComponentInParent<Loot>();
            EquipWeapon(loot.Item as Weapon);
            loot.Destroy();
        }

        if (other.CompareTag(Constants.Tags.c_Enemy))
        {
            var enemy = other.attachedRigidbody.GetComponent<EnemyController>();
            
            if(enemy.CanDamage)
                Hit(enemy.GetDamages());
        }
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        UpdateMovement();

        if(m_IsMoving)
        {
            if (m_HasNearestUnit)
            {
                if(!m_TargetIndicator.activeSelf)
                    m_TargetIndicator.gameObject.SetActive(true);

                m_TargetIndicator.transform.position = m_NearestUnit.transform.position;
            }
            else
                m_TargetIndicator.gameObject.SetActive(false);
        }
        else
        {
            if(m_HasTarget)
            {
                if (!m_TargetIndicator.activeSelf)
                    m_TargetIndicator.gameObject.SetActive(true);

                m_TargetIndicator.transform.position = m_Target.transform.position;
            }
            else if (m_HasNearestUnit)
            {
                //StartAttacking();
            }
            else
                m_TargetIndicator.gameObject.SetActive(false);
        }
    }

    private void OnLevelSpawned()
    {
        transform.position = LevelManager.Instance.PlayerOrigin.position;
        m_Graphics.rotation = LevelManager.Instance.PlayerOrigin.rotation;
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.RESET:
                m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
                m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
                if(m_Health != null)
                    m_Health.Destroy();
                m_Health.Init(m_MaxHealth + GameManager.Instance.PlayerLevel * 8);
                m_HasTarget = false;
                m_Target = null;
                m_TargetIndicator.gameObject.SetActive(false);
                ActivateHitBox(false);
                ActivateWeaponParticles(false);
                m_HasNearestUnit = false;

                var wd = ItemDropManager.Instance.StartingWepaons.PickRandomElementInList();
                var weapon = Instantiate(wd.m_Item);
                weapon.Init(wd);
                EquipWeapon(weapon as Weapon);
                break;
        }
    }

    private void OnGamePhaseEnded(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.GAME:
                m_TargetIndicator.gameObject.SetActive(false);
                //StopAttacking();
                StopMoving();
                break;
        }
    }

    private void OnUnitsUpdated(bool _HasUnits, EnemyController _Enemy)
    {
        m_HasNearestUnit = _HasUnits;

        if(_HasUnits)
        {
            m_NearestUnit = _Enemy;
        }
    }

    private void UpdateMovement()
    {
        m_JoyDir = m_Joystick.GetJoystickDirection();
        var isMoving = m_Joystick.IsTouching();

        if (isMoving)
        {
            if(!m_IsMoving)
            {
                StopAttacking();
                StartMoving();
            }

            Move();

           
        }
        else
        {
            if (m_IsMoving)
            {
                StopMoving();
                StartAttacking();
            }

            Attack();
        } 
    }

    private void StartAttacking()
    {
        if (!m_HasTarget)
        {
            if (m_HasNearestUnit)
            {
                m_HasTarget = true;
                m_Target = m_NearestUnit;

                var dir = m_Target.transform.position - transform.position;
                dir.y = 0;
                dir.Normalize();

                m_WorldDir = dir;

                m_Graphics.rotation = Quaternion.LookRotation(m_WorldDir, Vector3.up);
                
            }
        }
  
        if(!m_Animator.GetBool(Constants.AnimatorValues.c_IsAttacking))
            m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, true);
    }

    private void Attack()
    {
        /*if (!m_HasTarget)
            return;*/
    }

    private void StopAttacking()
    {
        m_Target = null;
        m_HasTarget = false;
        m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
        ActivateHitBox(false);
        ActivateWeaponParticles(false);
    }

    private void StartMoving()
    {
        m_IsMoving = true;
    }

    private void Move()
    {
        m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, m_JoyDir != Vector2.zero);

        if (m_JoyDir != Vector2.zero && !m_LockMovements)
        {
            m_Graphics.rotation = Quaternion.LookRotation(m_WorldDir, Vector3.up);

            var pos = transform.position;
            m_WorldDir = Vector3.ProjectOnPlane(new Vector3(m_JoyDir.x, 0, m_JoyDir.y), Vector3.up).normalized;
            var camDir = CameraManager.Instance.CameraDirection;
            var angleCam = Mathf.Atan2(camDir.z, camDir.x);
            var angleDir = Mathf.Atan2(m_WorldDir.z, m_WorldDir.x);
            var angle = angleDir + angleCam - Mathf.PI / 2;
            m_WorldDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;

            var dist = m_MoveSpeed * m_MovementSpeedMultiplier * Time.deltaTime;
            var dest = pos + m_WorldDir * dist;

            if (NavMesh.SamplePosition(dest, out m_NavMeshHit, dist, NavMesh.AllAreas))
            {
                transform.position = m_NavMeshHit.position;
            }
        }
            
    }

    private void StopMoving()
    {
        m_IsMoving = false;
        m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
    }

    private void OnEnemyDeath(EnemyController _Enemy)
    {
        if(_Enemy == m_Target)
        {
            m_HasTarget = false;
            m_Target = null;
            m_TargetIndicator.gameObject.SetActive(false);
        }
    }

    private void OnHit(bool _IsHit)
    {
        m_LockMovements = _IsHit;
    }

    private void ActivateHitBox(bool _isOn)
    {
        m_Hitbox.Collider.enabled = _isOn;
    }

    private void ActivateWeaponParticles(bool _isOn)
    {
        if (!m_HasWeaponEquipped || m_EquippedWeapon == null)
            return;

        if(_isOn)
            m_EquippedWeapon.SlashParticles.Play();
        else
            m_EquippedWeapon.SlashParticles.Stop();
    }

    private void EquipWeapon(Weapon _Weapon)
    {
        DestroyEquippedWeapon();

        m_EquippedWeapon = _Weapon;
        m_HasWeaponEquipped = true;

        var t = EquippedWeapon.transform;

        t.SetParent(m_WeaponSlot);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.Euler(0, 0, 0);
        t.localScale = Vector3.one;

        WeaponData wd = _Weapon.Data as WeaponData;

        SetSkin(wd.m_CharacterSkin);
        SetHitBox(wd.m_WeaponHitBox, wd.m_Damages, wd.m_CriticalChance);
        SetAttackSpeed(wd.m_AttackSpeed);
        SetMovementSpeedMultiplier(wd.m_MovementSpeedMultiplier);
        SetWeaponType(wd.m_WeaponType);

        m_Animator.SetTrigger("EquipWeapon");

        onWeaponEquipped?.Invoke(m_EquippedWeapon.Data);
    }

    private void DestroyEquippedWeapon()
    {
        if(m_HasWeaponEquipped)
        {
            Destroy(EquippedWeapon.gameObject);
            m_EquippedWeapon = null;
            m_HasWeaponEquipped = false;
        }
    }
    
    private void SetSkin(Texture _Texture)
    {
        m_PlayerRenderer.material.SetTexture(Constants.GameplayValues.c_BaseTexture, _Texture);
    }

    private void SetHitBox(Bounds _HitBoxBounds, int _Damages, float _CritChance)
    {
        m_Hitbox.Collider.size = _HitBoxBounds.size;
        m_Hitbox.Collider.center = _HitBoxBounds.center;

        m_Hitbox.SetDamages(_Damages, _CritChance);
    }

    private void SetAttackSpeed(float _Value)
    {
        m_AttackSpeedMultiplier = _Value;
        m_Animator.SetFloat(Constants.AnimatorValues.c_AttackSpeedMultiplier, m_AttackSpeedMultiplier);
    }

    private void SetMovementSpeedMultiplier(float _Value)
    {
        m_MovementSpeedMultiplier =  _Value;
        m_Animator.SetFloat(Constants.AnimatorValues.c_MovementSpeedMultiplier, m_MoveSpeed / c_MoveSpeedAnimation * m_MovementSpeedMultiplier);
    }

    private void SetWeaponType(int _Value)
    {
        m_WeaponType = _Value;
        m_Animator.SetInteger(Constants.AnimatorValues.c_WeaponType, m_WeaponType);
    }

    private void OnEndAttack()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            StopAttacking();
        else
            StartAttacking();
    }

    private IEnumerator InvincibilityRoutine()
    {
        yield return new WaitForSeconds(2f);
        m_Health.SetInvincibility(false);
    }

    private void Hit(int _Damages)
    {
        m_Health.AddHealth(-_Damages);
        var hitMarker = Instantiate(UnitManager.Instance.HitMarkerPrefab, transform.position, Quaternion.identity);
        hitMarker.Init(transform, _Damages, false, true);
        
        if(m_Health.Value > 0)
        {
            m_Health.SetInvincibility(true);
            m_Animator.SetTrigger(Constants.AnimatorValues.c_Hit);
            StartCoroutine(InvincibilityRoutine());
        }      
    }

    private void OnDeath()
    {
        m_Animator.SetTrigger(Constants.AnimatorValues.c_Death);
        FSMManager.Instance.ChangePhase(GamePhase.FAILURE);
    }
}
