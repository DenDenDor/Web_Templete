using System;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperInfoWindow : AbstractWindowUi
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _panel;
    
    public event Action Clicked;

    public override void Init()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Clicked?.Invoke();
    }

    public void OpenPanel()
    {
        _panel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        _panel.gameObject.SetActive(false);
    }
}