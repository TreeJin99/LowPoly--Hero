using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject trailEffectObj;
    public GameObject explosionEffectObj;
    public Rigidbody rigid;

    private AudioSource explodeSound;

    private void Start()
    {
        explodeSound = GetComponent<AudioSource>();
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        explodeSound.Play();

        // 속도 비활성화
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        meshObj.SetActive(false);
        trailEffectObj.SetActive(false);
        explosionEffectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hit in rayHits)
            hit.transform.GetComponent<Enemy_Controller>().HitByGrenade(transform.position);

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

}
