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
    const string WSkill = "WSkill";
    const string RSkill = "RSkill";

    public bool _isInside = false;
    public float pushForce = 8f;

    private CustomActions _input;
    private NavMeshAgent _agent;
    private Animator _anim;
    private float _lookRotationSpeed = 8f;
    private float _jumpSpeed = 8f;
    private float _pullForce = 5f;
    private bool _isDashing = false;
    private float _flashDistance = 5f;
    private bool _valueChanged = false;

    [SerializeField] private ParticleSystem _clickEffect;
    [SerializeField] private LayerMask _clickableLayers;
    [SerializeField] private GameObject _ward;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject _skillQ;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private MarketManager _marketManager;

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
        _input.Main.E_Skill.performed += ctx => SkillE();
        _input.Main.R_Skill.performed += ctx => SkillR();
        _input.Main.Ward.performed += ctx => Ward();
        _input.Main.Flash.performed += ctx => Flash();
        _input.Main.P_Market.performed += ctx => Market();
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
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Instantiate(_skillQ, _spawnPoint.position, _spawnPoint.rotation);
        }
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
    private void SkillE()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        _agent.isStopped = true;

        if (Physics.Raycast(ray, out hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pullDirection = _player.transform.position - hit.point;
                pullDirection.Normalize();
                rb.AddForce(pullDirection * _pullForce, ForceMode.Impulse);
            }
        }
        _agent.isStopped = false;
    }
    private void SkillR()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        _agent.isStopped = true;

        if (Physics.Raycast(ray, out hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = hit.point - _player.transform.position;
                pushDirection.Normalize();
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
        _agent.isStopped = false;
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
    private void Flash()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targerPoint = hit.point;
            Vector3 direction = (targerPoint - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * _flashDistance;
            transform.position = newPosition;
        }
    }
    private void Market()
    {
        _marketManager.MarketPanel();
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
        Items();
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
        if (_isDashing == true)
        {
            _anim.Play(WSkill);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _anim.Play(RSkill);
        }
    }
    private IEnumerator MovePlayerToPosition(Vector3 targetPosition)
    {
        _agent.isStopped = true;
        _isDashing = true;
        Vector3 startPosition = _player.transform.position;
        float lastTime = 0f;
        while (lastTime < 1f)
        {
            _player.transform.position = Vector3.Lerp(startPosition, targetPosition, lastTime);
            lastTime += _jumpSpeed * Time.deltaTime;
            yield return null;
        }
        _player.transform.position = targetPosition;
        _isDashing = false;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Market"))
        {
            _isInside = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Market"))
        {
            _isInside = false;
        }
    }
    private void Items()
    {
        if (_marketManager._doesAxeOfKratosHave == true && _valueChanged == false)
        {
            pushForce += 10 * pushForce / 100;
            _valueChanged = true;
        }
    }
}
