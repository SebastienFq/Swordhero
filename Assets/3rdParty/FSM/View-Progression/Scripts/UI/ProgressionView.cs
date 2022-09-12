using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class ProgressionView : View<ProgressionView>
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Show([CallerFilePath] string callerFilePath = "")
    {
        base.Show(callerFilePath);
    }
}