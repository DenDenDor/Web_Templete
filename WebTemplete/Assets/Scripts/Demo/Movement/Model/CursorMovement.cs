using UnityEngine;

public class CursorMovement : IMovement
{
    private Camera _mainCamera = Camera.main;
    private Vector3 _targetPosition;

    public Vector3 Move(Transform transform, float speed)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = _mainCamera.WorldToScreenPoint(transform.position).z;
        _targetPosition = _mainCamera.ScreenToWorldPoint(mousePosition);

        return Vector3.Lerp(transform.position, _targetPosition, speed * Time.deltaTime);
    }
}
