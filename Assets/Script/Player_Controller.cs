using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Animator _animator;
    private Camera _camera;
    private CharacterController _characterController;
    private Vector3 moveVelocity;

    public float walkSpeed = 2f;
    public float defendSpeed = 1f;
    public float runSpeed = 4f;
    public float smoothness = 10f;
    public float attackSpeed = 0f;
    public float jumpHeight = 300f;
    public float gravity = -20f;

    private float currentHP;
    private float currentSpeed;
    private float attackDelay;
    private float jumpSpeed = 15;

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

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GetInput();
        Movement();
        SetAnimator();
    }

    private void LateUpdate()
    {
        toggleCamera();
    }
    private void GetInput()
    {

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        altDown = Input.GetKey(KeyCode.LeftAlt);
        wDown = Input.GetKey(KeyCode.W);

        isJump = Input.GetKeyDown(KeyCode.Space);
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

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 moveDir = forward * vAxis + right * hAxis;

        _characterController.Move(moveDir.normalized * currentSpeed * Time.deltaTime);


    }

    private void SetAnimator()
    {
        float isMove = isRun ? 1 : wDown ? 0.5f : 0;

        _animator.SetFloat("Blend", isMove, 0.1f, Time.deltaTime);

        if (isDefend)
            _animator.SetTrigger("isDefend");
    }

}
