using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator m_Animator = null;
    [SerializeField] private Joystick m_Joystick = null;
    [SerializeField] private Transform m_Graphics = null;

    private float m_MoveSpeed = 5;
    private bool m_IsMoving;
    private NavMeshHit m_NavMeshHit;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.RESET:
                transform.position = GameManager.Instance.PlayerSpawnPoint.position;
                transform.rotation = GameManager.Instance.PlayerSpawnPoint.rotation;
                m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
                m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
                break;
        }
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var joyDir = m_Joystick.GetJoystickDirection();
        var isMoving = m_Joystick.IsTouching() && joyDir != Vector2.zero;

        if (isMoving)
        {
            if(!m_IsMoving)
            {
                m_IsMoving = true;
                m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
                m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, true);
            }

            var pos = transform.position;

            var worldDir = Vector3.ProjectOnPlane(new Vector3(joyDir.x, 0, joyDir.y), Vector3.up).normalized;
            var dist = m_MoveSpeed * Time.deltaTime;
            var dest = pos + worldDir * dist;

            if (NavMesh.SamplePosition(dest, out m_NavMeshHit, dist, NavMesh.AllAreas))
            {
                transform.position = m_NavMeshHit.position;
                m_Graphics.rotation = Quaternion.LookRotation(worldDir, Vector3.up);
            }
        }
        else
        {
            if (m_IsMoving)
            {
                m_IsMoving = false;
                m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
                m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, true);
            }
        } 
    }
}
