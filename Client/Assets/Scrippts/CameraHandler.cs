using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    //���������������
    public Transform targetTransform;
    //����������
    public Transform cameraTransform;
    //��������������Ҫ����x����ת��������Ұ�����°ڶ�
    public Transform cameraPivotTransform;
    //���ϲ�����������Pivot����������������������Ҫ����y����ת��������Ұ�����Ұڶ�
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public LayerMask ignoreLayers;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35f;
    public float maxmumPivot = 35f;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake()
    {
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        //ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        Vector3 _targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = _targetPosition;

        HandleCameraCollision(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        //������Ұڶ��ĽǶ�
        lookAngle += (mouseXInput * lookSpeed) / delta;
        //������°ڶ��ĽǶ�
        pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maxmumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCameraCollision(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        //��Pivotָ������ķ���
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        //���ľ����ǽű�����ʱ�����Pivot�ľ��룬����������ǲ���ģ�
        //Ҳ����˵����������������⵽�����壬Ҳ����˵��������֮�����һ�����壬
        //��ʱ Ӧ��������ƶ�����������ǰ�棬��û������ʱ�������Ӧ�ûָ�����Ϸ����ʱ��Pivot�ľ���
        if (Physics.SphereCast
            (cameraPivotTransform.position, cameraSphereRadius, direction,
            out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);
        }

        //��ֹ�������������һ��
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        //ֻ�ı�������������z����룬Ҳ�����������Զ��
        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.02f);
        cameraTransform.localPosition = cameraTransformPosition;
    }
}