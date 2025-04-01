using System.Collections;
using UnityEngine;

public class DeveloperInfoRouter : IRouter
{
    private DeveloperInfoWindow Window => UiController.Instance.GetWindow<DeveloperInfoWindow>();

    private int _waitTime = 3;
    
    public void Init()
    {
        Window.Clicked += OpenDeveloperPanel;
    }

    private void OpenDeveloperPanel()
    {
        Window.OpenPanel();

        Window.StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_waitTime);
        
        Window.ClosePanel();
    }

    public void Exit()
    {
        
    }
}