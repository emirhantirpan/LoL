using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShot : MonoBehaviour
{
    private float _speed = 20f;
    private float _maxDistance = 20f;
    private Vector3 _startPosition;
    private Rigidbody _rb;
    private Transform _playerTransform;


    private void Start()
    {
        _startPosition = transform.position;
        _rb = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);

        if (Vector3.Distance(_startPosition,transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }

    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            _playerTransform.position = col.transform.position;
            Destroy(gameObject);
        }
        if (col.gameObject.CompareTag("SomethingElse"))
        {
            _rb.isKinematic = true;
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("SomethingElse"))
        {
            _rb.isKinematic = false;
        }
    }
}
