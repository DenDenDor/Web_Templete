using UnityEngine;

public class MovementWindow : AbstractWindowUi
{
    [SerializeField] private Transform _player;

    public int Speed { get; private set; } = 5;

    public Transform Player => _player;

    public override void Init()
    {
        
    }

    public void UpdatePosition(Vector3 position)
    {
        _player.position = position;
    }
}