using System;
using UnityEngine;
using UnityEngine.UI;

public class StartGameWindow : AbstractWindowUi
{
    [SerializeField] private Button _button;

    public event Action Clicked;
    
    public override void Init()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Clicked?.Invoke();
    }
}