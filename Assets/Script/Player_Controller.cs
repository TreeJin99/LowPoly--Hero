using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Animator _animator;
    private Camera _camera;
    private CharacterController _characterController;

    // 플레이어 기본 스탯
    public float playerDamage = 20f;
    public float walkSpeed = 2f;
    public float defendSpeed = 1f;
    public float runSpeed = 4f;
    public float jumpHeight = 3f;

    // 플레이어 무기 & 방어구
    public Collider weaponCollider;
    public Collider shieldCollider;
    public TrailRenderer trailEffect;
    public float attackSpeed = 0.4f;

    // 플레이어 움직임 시스템 설정 
    public float smoothness = 10f;
    public float gravity = 10f;

    private float currentHP;
    private float currentSpeed;
    private float attackDelay;

    private Vector3 moveDir;

    private float hAxis;
    private float vAxis;
    private float xMouse;
    private float yMouse;


    private bool wDown;
    private bool altDown;

    private bool isJump;
    private bool isRun;
    private bool isAttack;
    private bool isDefend;
    private bool isCoolTime;

    private void Awake()
    {
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        moveDir = Vector3.zero;
    }

    private void Update()
    {
        GetInput();
        Movement();
        Action();
    }

    private void LateUpdate()
    {
        toggleCamera();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;


        if (hit.gameObject.tag == "Enemy")
            Debug.Log("적팀!");

        Debug.Log("충돌했으!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            Debug.Log("아이템!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Item"))
        {
            Debug.Log("아이템!!");
        }
        else if(collision.collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적팀!");
        }

        if (collision.gameObject.tag == "Item")
            Debug.Log("ㅇㄴㅁㅇㄴㅁ");
    }

    private void GetInput()
    {

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        altDown = Input.GetKey(KeyCode.LeftAlt);
        wDown = Input.GetKey(KeyCode.W);

        isJump = Input.GetKey(KeyCode.Space);
        isRun = Input.GetKey(KeyCode.LeftShift);

        isAttack = Input.GetMouseButtonDown(0);
        isDefend = Input.GetMouseButton(1);
    }

    private void toggleCamera()
    {
        if (!altDown)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    private void Movement()
    {
        currentSpeed = isDefend ? defendSpeed : isRun ? runSpeed : walkSpeed;
        if (_characterController.isGrounded)
        {
            moveDir = new Vector3(hAxis, 0, vAxis);
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= currentSpeed;

            if (isJump)
                moveDir.y = jumpHeight;
        }

        moveDir.y -= gravity * Time.deltaTime;
        _characterController.Move(moveDir * Time.deltaTime);

        float isMove = isRun ? 1 : wDown ? 0.5f : 0;
        _animator.SetFloat("Blend", isMove, 0.1f, Time.deltaTime);

    }

    private void Action()
    {
        attackDelay += Time.deltaTime;
        isCoolTime = attackDelay + attackSpeed < 1f;

        if (isAttack && !isCoolTime && !isDefend)
        {
            StopCoroutine("Attack");
            StartCoroutine("Attack");
            _animator.SetTrigger("isAttack");
            attackDelay = 0f;
        }
        else if (!isAttack && isDefend)
        {
            shieldCollider.enabled = true;
            _animator.SetTrigger("isDefend");
        }
    }


    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(0.3f);
        weaponCollider.enabled = false;
    }



}
