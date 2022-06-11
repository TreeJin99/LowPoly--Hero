using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    public float maxHP;
    public Transform _target;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Renderer _renderer;
    private Color originColor;

    private float currentHP;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();
        originColor = _renderer.material.color;

        currentHP = maxHP;
    }

    private void Update()
    {

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack")
        {
            //Player_Controller pc = other.GetComponent<Player_Controller>();
            //Debug.Log(pc.playerDamage + "의 체력이 감소!");
            //currentHP -= pc.playerDamage;

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

            Destroy(gameObject, 3);
        }
    }

}
