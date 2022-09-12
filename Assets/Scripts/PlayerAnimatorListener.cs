using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorListener : MonoBehaviour
{
    public Action<bool> onDamage;
    public Action<bool> onEffect;

    public void StartDamage()
    {
        onDamage?.Invoke(true);
    }

    public void EndDamage()
    {
        onDamage?.Invoke(false);
    }

    public void StartSlashEffectPhase()
    {
        onEffect?.Invoke(true);
    }

    public void EndSlashEffectPhase()
    {
        onEffect?.Invoke(false);
    }
}
