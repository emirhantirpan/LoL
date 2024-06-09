using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    const string Idle = "Idle";
    const string Running = "Running";

    private CustomActions _input;
    private NavMeshAgent _agent;
    private Animator _anim;
    private float _lookRotationSpeed = 8f;

    [SerializeField] private ParticleSystem _clickEffect;
    [SerializeField] private LayerMask _clickableLayers;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        _input = new CustomActions();
        AssignInputs();
    }
    void AssignInputs()
    {
        _input.Main.Move.performed += ctx => ClickToMove();
    }
    void ClickToMove()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
        {
            _agent.destination = hit.point;
            if (_clickEffect != null)
            {
                Instantiate(_clickEffect, hit.point += new Vector3(0f, 0.1f, 0f), _clickEffect.transform.rotation);
            }
        }
    }
    void OnEnable()
    {
        _input.Enable();
    }
    void OnDisable()
    {
        _input.Disable();
    }
    private void Update()
    {
        FaceTarget();
        SetAnimations();
    }
    private void FaceTarget()
    {
        Vector3 direction = (_agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lookRotationSpeed);
    }
    private void SetAnimations()
    {
        if (_agent.velocity == Vector3.zero)
        {
            _anim.Play(Idle);
        }
        else
        {
            _anim.Play(Running);
        }
    }
}
