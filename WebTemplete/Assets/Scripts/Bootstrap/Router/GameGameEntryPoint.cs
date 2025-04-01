using System.Collections.Generic;
using UnityEngine;

public class GameGameEntryPoint : AbstractGameEntryPoint
{
    protected override List<IRouter> Routers => new List<IRouter>()
    {
        new MovementRouter(),
        new PlayerTriggerRouter(),
        new CoinRouter(),
    };
}