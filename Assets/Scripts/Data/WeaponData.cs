using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ItemData
{
    public Texture m_CharacterSkin;
    public Bounds m_WeaponHitBox;
    public float m_AttackSpeed = 1;
    public int m_Damages = 100;
    [Range(0, 1)] public float m_CriticalChance = 0.1f;
    public int m_WeaponType = 0;
    public float m_MovementSpeedMultiplier = 1;

}
