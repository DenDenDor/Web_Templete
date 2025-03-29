using System;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    private Dictionary<Type, AbstractWindowUi> _windows = new Dictionary<Type, AbstractWindowUi>();
    
    private static UiController _instance;

    public static UiController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UiController>();
                if (_instance == null)
                {
                    throw new NotImplementedException("UiController not found!");
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        RegisterAllWindows();
    }

    private void RegisterAllWindows()
    {
        var allWindows = GetComponentsInChildren<AbstractWindowUi>(true);
        foreach (var window in allWindows)
        {
            var windowType = window.GetType();
            if (!_windows.ContainsKey(windowType))
            {
                _windows.Add(windowType, window);
            }
        }
    }

    public T GetWindow<T>() where T : AbstractWindowUi
    {
        if (_windows.TryGetValue(typeof(T), out var window))
        {
            return (T)window;
        }
        return null;
    }
}