using UnityEngine;

public class PlayerTriggerWindow : AbstractWindowUi
{
    [SerializeField] private PlayerTrigger _playerTrigger;

    public PlayerTrigger Trigger => _playerTrigger;

    public override void Init()
    {
        
    }
}