using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    public enum Type { A, B, C };
    public Type enemyType;

    public float maxHP;
    private float currentHP;
    private Transform _target;

    public bool isChase;
    public bool isAttack;
    public Collider attackCollider;
    public GameObject bullet;

    private NavMeshAgent _agent;
    private Rigidbody _rigidbody;
    private Renderer _renderer;
    private Color originColor;
    private Animator _animator;


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
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 0.5f;
                targetRange = 1f;
                break;

            case Type.B:
                targetRadius = 0.35f;
                targetRange = 31f;
                break;

            case Type.C:
                targetRadius = 0.2f;
                targetRange = 20f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius,
            transform.forward,
            targetRange,
            LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        _animator.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                attackCollider.enabled = true;

                yield return new WaitForSeconds(1f);
                attackCollider.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.5f);
                _rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);
                attackCollider.enabled = true;

                yield return new WaitForSeconds(0.5f);
                _rigidbody.velocity = Vector3.zero;
                attackCollider.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                Vector3 bulletPost = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                for (int i = 0; i < 3; i++)
                {
                    GameObject bulletInstant = Instantiate(bullet, bulletPost, transform.rotation);
                    Rigidbody bulletRigid = bulletInstant.GetComponent<Rigidbody>();
                    bulletRigid.velocity = transform.forward * 5f;

                    Destroy(bulletInstant, 5f);

                    yield return new WaitForSeconds(0.2f);
                }

                yield return new WaitForSeconds(1f);
                break;
        }

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
