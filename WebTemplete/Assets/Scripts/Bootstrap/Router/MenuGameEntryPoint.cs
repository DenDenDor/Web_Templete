using System.Collections.Generic;
using UnityEngine;

public class MenuGameEntryPoint : AbstractGameEntryPoint
{
    protected override List<IRouter> Routers => new List<IRouter>()
    {
        new StartGameRouter(),
        new DeveloperInfoRouter()
    };
}