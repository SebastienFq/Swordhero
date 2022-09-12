using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndView<T> : View<T> where T : EndView<T>
{
    // Buffer
    private Coroutine m_ShowCoroutine;
    public override void Show([CallerFilePath] string callerFilePath = "")
    {
        if (m_ShowCoroutine != null)
            StopCoroutine(m_ShowCoroutine);
        m_ShowCoroutine = StartCoroutine(DelayedShow());
    }
    private IEnumerator DelayedShow()
    {
        yield return new WaitForSeconds(0.8f);
        base.Show();
    }
    public virtual void OnContinueButton()
    {
        FSMManager.Instance.ChangePhase(GamePhase.RESET);
    }
}
