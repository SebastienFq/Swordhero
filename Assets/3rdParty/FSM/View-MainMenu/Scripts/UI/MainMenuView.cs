using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;
using MoreMountains.NiceVibrations;

public class MainMenuView : View<MainMenuView>
{
	public TextMeshProUGUI m_LevelText;

	private bool m_IsShown = false;

	public void Show(int _Level)
	{
		m_IsShown = true;
		
		base.Show();

		if (m_LevelText != null)
		{
			m_LevelText.text = "Level " + (_Level).ToString();
		}
	}

	public override void Hide()
	{
		base.Hide();

		m_IsShown = false;
	}

	protected override void Update()
	{
		base.Update();
		
		if (m_IsShown == false)
			return;
	}
}
