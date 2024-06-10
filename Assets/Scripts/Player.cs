using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    const string Idle = "Idle";
    const string Run = "Run";

    private CustomActions _input;
    private NavMeshAgent _agent;
    private Animator _anim;
    private float _lookRotationSpeed = 8f;
    private float _jumpSpeed = 8f;

    [SerializeField] private ParticleSystem _clickEffect;
    [SerializeField] private LayerMask _clickableLayers;
    [SerializeField] private GameObject _ward;
    [SerializeField] private GameObject _player;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        _input = new CustomActions();
        AssignInputs();
    }
    private void AssignInputs()
    {
        _input.Main.Move.performed += ctx => ClickToMove();
        _input.Main.Stop.performed += ctx => StopMove();
        _input.Main.Q_Skill.performed += ctx => SkillQ();
        _input.Main.W_Skill.performed += ctx => SkillW();
        _input.Main.R_Skill.performed += ctx => SkillR();
        _input.Main.Ward.performed += ctx => Ward();
    }
    private void ClickToMove()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
        {
            _agent.isStopped = false;
            _agent.destination = hit.point;
            if (_clickEffect != null)
            {
                Instantiate(_clickEffect, hit.point += new Vector3(0f, 0.1f, 0f), _clickEffect.transform.rotation);
            }
        }
    }
    private void StopMove()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
    }
    private void SkillQ()
    {

    }
    private void SkillW()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ward"))
            {
                StartCoroutine(MovePlayerToPosition(hit.point));
            }
        }
    }
    private void SkillR()
    {

    }
    private void Ward()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Instantiate(_ward, hit.point, Quaternion.identity);
            }
        }
    }
    private void OnEnable()
    {
        _input.Enable();
    }
    private void OnDisable()
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
            _anim.Play(Run);
        }
    }
    private IEnumerator MovePlayerToPosition(Vector3 targetPosition)
    {
        _agent.isStopped = true;
        Vector3 startPosition = _player.transform.position;
        float lastTime = 0f;
        while (lastTime < 1f)
        {
            _player.transform.position = Vector3.Lerp(startPosition, targetPosition, lastTime);
            lastTime += _jumpSpeed * Time.deltaTime;
            yield return null;
        }
        _player.transform.position = targetPosition;

    }
}
