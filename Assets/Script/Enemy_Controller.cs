using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    public float maxHP;
    private Transform _target;
    public bool isChase;
    public bool isAttack;
    public Collider attackCollider;


    private NavMeshAgent _agent;
    private Rigidbody _rigidbody;
    private Renderer _renderer;
    private Color originColor;
    private Animator _animator;

    private float currentHP;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        originColor = _renderer.material.color;
        _animator = GetComponentInChildren<Animator>();

        _target = GameObject.Find("DogPolyart").transform;

        currentHP = maxHP;

        Invoke("ChaseStart", 2);
    }

    private void Update()
    {
        if (_agent.enabled)
        {
            _agent.SetDestination(_target.position);
            _agent.isStopped = !isChase;
        }
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void Targeting()
    {
        float targetRadius = 0.5f;
        float targetRange = 0.5f;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius,
            transform.forward,
            targetRange,
            LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;

        _animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.2f);
        attackCollider.enabled = true;

        yield return new WaitForSeconds(1f);
        attackCollider.enabled = false;


        yield return new WaitForSeconds(1f);
        isChase = true;
        isAttack = false;
        _animator.SetBool("isAttack", false);
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

        }
    }

    private void ChaseStart()
    {
        isChase = true;
        _animator.SetBool("isWalk", true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack")
        {

            Player_Controller pc = Player_Controller.PLAYER_INSTANCE;
            currentHP -= pc.playerDamage;
            Vector3 knockBack = transform.position - other.transform.position;
            StartCoroutine(OnDamage(knockBack));
        }
    }


    IEnumerator OnDamage(Vector3 knockBack)
    {
        _renderer.material.color = Color.red;

        knockBack = knockBack.normalized;
        knockBack += Vector3.up;
        _rigidbody.AddForce(knockBack * 1.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.3f);
        if (currentHP > 0)
        {
            _renderer.material.color = originColor;
        }
        else
        {
            _renderer.material.color = Color.gray;
            gameObject.layer = 12;

            isChase = false;
            _agent.enabled = false;
            _animator.SetTrigger("doDie");

            Destroy(gameObject, 3);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        Debug.Log("Ãæµ¹!!");
        currentHP -= 100;

        Vector3 knockBack = transform.position - explosionPos;
        StartCoroutine(OnDamage(knockBack));
    }

}
