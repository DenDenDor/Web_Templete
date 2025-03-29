using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameEntryPoint : MonoBehaviour
{
    protected abstract IEnumerable<IRouter> Routers { get; }

    private void Start()
    {
        InitWindows();
        InitRouter();
    }

    private void OnDisable()
    {
        ExitRouter();
    }

    private void InitWindows()
    {
        foreach (var window in FindObjectsOfType<AbstractWindowUi>(true))
        {
            window.Init();
        }
    }

    private void InitRouter()
    {
        foreach (var router in Routers)
        {
            router.OnInit();
        }
    }

    private void ExitRouter()
    {
        foreach (var router in Routers)
        {
            router.OnExit();
        }
    }
}
