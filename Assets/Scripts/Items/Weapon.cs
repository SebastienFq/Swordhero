using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [SerializeField] private ParticleSystem m_SlashParticles;

    public ParticleSystem SlashParticles => m_SlashParticles;
}
