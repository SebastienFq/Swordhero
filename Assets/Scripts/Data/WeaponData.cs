using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ItemData
{
    public Bounds m_WeaponHitBox;
    public float m_AttackSpeed = 1;
    public int m_Damages = 100;
    public int m_WeaponType = 0;
    public float m_MovementSpeedMultiplier = 1;
}
