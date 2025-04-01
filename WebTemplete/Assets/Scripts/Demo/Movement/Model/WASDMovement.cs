using UnityEngine;

public class WASDMovement : IMovement
{
    private Vector3 _targetPosition;

    private float smoothTime = 0.0005f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;

    public Vector3 Move(Transform transform, float speed)
    {
        Vector3 inputDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) inputDirection.y += 1;
        if (Input.GetKey(KeyCode.S)) inputDirection.y -= 1;
        if (Input.GetKey(KeyCode.A)) inputDirection.x -= 1;
        if (Input.GetKey(KeyCode.D)) inputDirection.x += 1;

        if (inputDirection.magnitude > 1)
        {
            inputDirection.Normalize();
        }

        targetPosition = transform.position + inputDirection * speed * Time.deltaTime;
        
        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
