using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Transform objectToFollow;
    public Transform realCamera;
    private Vector3 dirNomalized;
    private Vector3 finalDir;

    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 70f;
    public float smoothness = 10f;

    public float minDistance = 1f;
    public float maxDistance = 3f;
    private float finalDistnace;

    private float rotX;
    private float rotY;

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNomalized = realCamera.localPosition.normalized;
        finalDistnace = realCamera.localPosition.magnitude;
    }

    private void Update()
    {
        GetInput();
        CameraAngle();
    }

    private void LateUpdate()
    {
        CameraMovement();
    }

    private void GetInput()
    {
        // �����¿� ������ ���� �����Ѵ�.
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
    }



    private void CameraAngle()
    {
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    private void CameraMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);
        finalDir = transform.TransformPoint(dirNomalized * maxDistance);

        RaycastHit hit;

        // �÷��̾�� ī�޶� ���̿� ��ü�� ���� ���
        if (Physics.Linecast(transform.position, finalDir, out hit))
            finalDistnace = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        else
            finalDistnace = maxDistance;

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNomalized * finalDistnace, Time.deltaTime * smoothness);
    }

}
