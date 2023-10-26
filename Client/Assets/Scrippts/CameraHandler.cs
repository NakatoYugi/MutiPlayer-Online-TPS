using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    //相机跟随的物体组件
    public Transform targetTransform;
    //相机物体组件
    public Transform cameraTransform;
    //相机父级组件，主要控制x轴旋转，就是视野的上下摆动
    public Transform cameraPivotTransform;
    //最上层物体的组件，Pivot和相机都是它的子组件，主要控制y轴旋转，就是视野的左右摆动
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
        //相机左右摆动的角度
        lookAngle += (mouseXInput * lookSpeed) / delta;
        //相机上下摆动的角度
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
        //由Pivot指向相机的方向
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        //检测的距离是脚本唤醒时相机与Pivot的距离，这个检测距离是不变的；
        //也就是说，假如以这个距离检测到了物体，也就是说人物和相机之间夹了一个物体，
        //此时 应该让相机移动到这个物体的前面，当没有物体时，相机又应该恢复到游戏启动时与Pivot的距离
        if (Physics.SphereCast
            (cameraPivotTransform.position, cameraSphereRadius, direction,
            out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);
        }

        //防止相机与人物贴在一起
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        //只改变相机本地坐标的z轴距离，也就是与人物的远近
        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.02f);
        cameraTransform.localPosition = cameraTransformPosition;
    }
}