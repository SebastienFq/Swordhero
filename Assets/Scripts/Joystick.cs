using UnityEngine;

public class Joystick : MonoBehaviour
{
    public float m_Amplitude;
    public float m_DeadZone;
    public bool m_ResetOnClick;
    public RectTransform m_ContainerTr;
    public RectTransform m_DotTr;

    // Cache
    private RectTransform m_Transform;

    // Buffers
    private bool m_IsTouching;
    private Vector2 m_StartPosition;
    private Vector2 m_Position;
    private Vector2 m_Direction;

    private void Awake()
    {
        // Cache
        m_Transform = GetComponent<RectTransform>();

        // Buffers
        m_IsTouching = false;
        m_StartPosition = m_ContainerTr.anchoredPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Transform, Input.mousePosition, null, out uiPos);
            m_IsTouching = true;

            Vector2 diffFromStart = uiPos - m_StartPosition;
            float diffFromStartMag = diffFromStart.magnitude;

            if (diffFromStartMag < m_Amplitude && !m_ResetOnClick)
            {
                Vector2 normDiff = diffFromStart.normalized;

                m_DotTr.localPosition = diffFromStart;
                m_Position = normDiff * (diffFromStartMag / m_Amplitude);

                if (diffFromStartMag < m_DeadZone)
                    m_Direction = Vector2.zero;
                else
                    m_Direction = normDiff;
            }
            else
            {
                m_ContainerTr.anchoredPosition = uiPos;
                m_Position = Vector2.zero;
                m_Direction = Vector2.zero;
            }
        }

        if (m_IsTouching)
        {
            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Transform, Input.mousePosition, null, out uiPos);

            Vector2 diffFromStart = uiPos - m_ContainerTr.anchoredPosition;
            Vector2 normDiff = diffFromStart.normalized;
            float diffFromStartMag = diffFromStart.magnitude;

            if (diffFromStartMag < m_Amplitude)
            {
                m_DotTr.localPosition = diffFromStart;
                m_Position = normDiff * (diffFromStartMag / m_Amplitude);
            }
            else
            {
                m_ContainerTr.anchoredPosition += normDiff * (diffFromStartMag - m_Amplitude);
                m_DotTr.localPosition = normDiff * m_Amplitude;
                m_Position = normDiff;
            }

            if (diffFromStartMag < m_DeadZone)
                m_Direction = Vector2.zero;
            else
                m_Direction = normDiff;
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_IsTouching = false;
            m_ContainerTr.anchoredPosition = m_StartPosition;
            m_DotTr.localPosition = Vector3.zero;
            m_Position = Vector2.zero;
            m_Direction = Vector2.zero;
        }
    }

    public Vector2 GetJoystickPosition()
    {
        return m_Position;
    }

    public Vector2 GetJoystickDirection()
    {
        return m_Direction;
    }

    public bool IsTouching()
    {
        return m_IsTouching;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_ContainerTr == null)
            return;

        Gizmos.color = Color.blue;
        Vector3 amplitudeOffset = Vector3.right * m_Amplitude;
        amplitudeOffset = transform.TransformVector(amplitudeOffset);
        Gizmos.DrawWireSphere(m_ContainerTr.position, amplitudeOffset.x);

        Gizmos.color = Color.red;
        Vector3 deadZoneOffset = Vector3.right * m_DeadZone;
        deadZoneOffset = transform.TransformVector(deadZoneOffset);
        Gizmos.DrawWireSphere(m_ContainerTr.position, deadZoneOffset.x);
    }
#endif
}
