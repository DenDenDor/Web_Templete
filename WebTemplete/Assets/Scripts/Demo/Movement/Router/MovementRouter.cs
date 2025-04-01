using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementRouter : IRouter
{
    private Coroutine _coroutine;

    private readonly List<IMovement> _movements = new List<IMovement>()
    {
        new CursorMovement(),
        new WASDMovement()
    };
    
    private IMovement _currentMovement;

    private MovementWindow Window => UiController.Instance.GetWindow<MovementWindow>();


    public void Init()
    {
        _currentMovement = _movements.FirstOrDefault(x => x is WASDMovement);
        
        _coroutine = Window.StartCoroutine(Update());
    }

    private IEnumerator Update()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _currentMovement = _movements.FirstOrDefault(x => x is CursorMovement);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _currentMovement = _movements.FirstOrDefault(x => x is WASDMovement);
            }

            Window.UpdatePosition(_currentMovement.Move(Window.Player, Window.Speed));
            
            yield return null;
        }   
    }

    public void Exit()
    {
        Window.StopCoroutine(_coroutine);
    }
}