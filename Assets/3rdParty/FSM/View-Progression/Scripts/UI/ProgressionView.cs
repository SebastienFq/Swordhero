using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Collections;

public class ProgressionView : View<ProgressionView>
{
    public TextMeshProUGUI m_LevelText;

    [SerializeField] private RectTransform m_WeaponSlot;
    [SerializeField] private Image m_WeaponIcon;
    [SerializeField] private TextMeshProUGUI m_WeaponName;

    private IEnumerator weaponScaleRoutine;

    private void OnEnable()
    {
        PlayerController.onWeaponEquipped += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        PlayerController.onWeaponEquipped -= OnWeaponEquipped;
    }

    private void OnWeaponEquipped(ItemData _Item)
    {
        m_WeaponIcon.sprite = _Item.m_ItemIcon;
        m_WeaponName.text = _Item.m_ItemName;

        BumpWeaponSlot();
    }

    private void BumpWeaponSlot()
    {
        if (weaponScaleRoutine != null)
            StopCoroutine(weaponScaleRoutine);
        weaponScaleRoutine = WeaponScaleRoutine();
        StartCoroutine(weaponScaleRoutine);
    }

    private IEnumerator WeaponScaleRoutine()
    {
        yield return StartCoroutine(ExtensionMethodsUI.RescaleGraphicsRoutine(m_WeaponSlot, Vector3.one * 1.4f, 0.1f, CalculationEasing.EaseOutSine));
        StartCoroutine(ExtensionMethodsUI.RescaleGraphicsRoutine(m_WeaponSlot, Vector3.one * 1f, 0.1f, CalculationEasing.EaseInSine));
    }

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