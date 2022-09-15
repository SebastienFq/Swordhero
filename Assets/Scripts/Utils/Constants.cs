using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class Tags
    {
        public const string c_WeaponHitbox = "WeaponHitbox";
        public const string c_LootWeapon = "LootWeapon";
        public const string c_Enemy = "Enemy";
    }

    public static class Layers
    {
        
    }

    public static class GameplayValues
    {
        public const string c_PlayerLevel = "PlayerLevel";
        public const string c_BaseTexture = "_BaseMap";
        public const string c_EmissiveColor = "_EmissionColor";
    }

    public static class AnimatorValues
    {
        public const string c_IsMoving = "IsMoving";
        public const string c_IsAttacking = "IsAttacking";
        public const string c_MovementSpeedMultiplier = "MovementSpeedMultiplier";
        public const string c_AttackSpeedMultiplier = "AttackSpeedMultiplier";
        public const string c_WeaponType = "WeaponType";
        public const string c_Hit = "Hit";
        public const string c_Death = "Death";
    }
}
