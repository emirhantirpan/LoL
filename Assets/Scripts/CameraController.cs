using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;

    private float _smoothSpeed = 8f;

    [SerializeField] private Transform _target;

    private void Update()
    {
        if (_target == null)
        {
            return; 
        }

        Vector3 desiredPosition = new Vector3(_target.position.x + offset.x, _target.position.y + offset.y, _target.position.z + offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
