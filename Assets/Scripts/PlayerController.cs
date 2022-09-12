using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick m_Joystick = null;
    [SerializeField] private Transform m_Graphics = null;

    private float m_MoveSpeed = 5;
    private NavMeshHit m_NavMeshHit;

    private void Update()
    {
        var pos = transform.position;
        var joyDir = m_Joystick.GetJoystickDirection();
        var worldDir = Vector3.ProjectOnPlane(new Vector3(joyDir.x, 0, joyDir.y), Vector3.up).normalized;
        var dist = m_MoveSpeed * Time.deltaTime;
        var dest = pos + worldDir * dist;
        
        if(NavMesh.SamplePosition(dest, out m_NavMeshHit, dist, NavMesh.AllAreas))
        {
            transform.position = m_NavMeshHit.position;
            m_Graphics.rotation = Quaternion.LookRotation(worldDir, Vector3.up);
        }
    }
}
