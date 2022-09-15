using System.Collections;
using TMPro;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_Text;
    [SerializeField] private float _BaseSize = 2;
    [SerializeField] private float _CriticalSize = 3;
    [SerializeField] private AnimationCurve _Curve;

    private Transform m_Target;

    public void Init(Transform _Target, int _Damages, bool _IsCritical, bool _IsPlayer = false)
    {
        m_Text.color = _IsPlayer ? Color.red : (_IsCritical ? new Color(1, 0.3f, 0, 1) : Color.yellow);
        m_Text.text = _Damages.ToString() + (_IsCritical ? "!" : "");
        m_Text.fontSize = _IsCritical ? _CriticalSize : _BaseSize;
        m_Target = _Target;
        StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        float timer = 0;
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Quaternion startRot = m_Text.transform.localRotation;
        var camDir = CameraManager.Instance.CameraDirection;
        var side = (Random.value < 0.5 ? -1 : 1);
        Vector3 dir  = Vector3.Cross(camDir * side, Vector3.up).normalized;
        var color = m_Text.color;
        Vector3 lastPosition = m_Target.position + Vector3.up * 2.5f * m_Target.transform.localScale.x;
        while (timer < 1)
        {
            if(m_Target != null)
                lastPosition = m_Target.position + Vector3.up * 2.5f * m_Target.transform.localScale.x;

            transform.position = lastPosition + dir * timer / 2 + Vector3.up * 0.4f * _Curve.Evaluate(timer);
            timer += Time.deltaTime / 0.4f;
            color.a = Mathf.Clamp01(0.7f + (1 - timer));
            m_Text.color = color;
            m_Text.transform.localRotation = startRot * Quaternion.Euler(0 , 0 , side * timer * 15);
            transform.localScale = startScale + Vector3.one * Mathf.Sin(timer * Mathf.PI) * 0.4f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
