using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private BoxCollider m_Collider;

    public int Damages { get; private set; }
    public BoxCollider Collider => m_Collider;

    public void SetDamages(int _Value)
    {
        Damages = _Value;
    }
}
