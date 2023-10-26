using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraTransform;
    InputHandler inputHandler;
    PlayerManager playerManager;
    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public PlayerAnimatorManager animatorHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    float groundDirectionRayDistance = 0.2f;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField]
    float walkingSpeed = 1f;
    [SerializeField]
    float movementSpeed = 5f;
    [SerializeField]
    float sprintSpeed = 7f;
    [SerializeField]
    float rotationSpeed = 10f;
    [SerializeField]
    float fallingSpeed = 45;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<PlayerAnimatorManager>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();

        playerManager.isGrounded = true;
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }

    #region Movement
    Vector3 normalVector;
    Vector3 targetPosition;
    private void HandleRotation(float delta)
    {
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputHandler.moveAmount;

        targetDir = cameraTransform.forward * inputHandler.vertical; //�����˳��� �� ������������
        targetDir += cameraTransform.right * inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        //û���ƶ�������ʱ����ת���ֲ���
        if (targetDir == Vector3.zero)
            targetDir = myTransform.forward;

        float rs = rotationSpeed;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
        myTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        if (playerManager.isInteracting) return;

        moveDirection = cameraTransform.forward * inputHandler.vertical;
        moveDirection += cameraTransform.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;

        if (inputHandler.moveAmount > 0.5f)
        {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
        }
        else
        {
            if (inputHandler.moveAmount < 0.5f)
            {
                speed = walkingSpeed;
            }
            playerManager.isSprinting = false;
        }
        moveDirection *= speed;


        //TODO:���ƶ����������ͶӰ��ĳ��ƽ���ϣ�Ӱ������ٶȣ���¥��ʱ�ٶȻ����
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0f, playerManager.isSprinting);

        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 orgin = myTransform.position;
        orgin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(orgin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        //����ʱ�������º���ǰ����
        if (playerManager.isInAir)
        {
            rigidbody.AddForce(Vector3.down * fallingSpeed);
            rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        orgin += dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        Debug.DrawRay(orgin, Vector3.down * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        //���¼��Ӵ���
        if (Physics.Raycast(orgin, Vector3.down, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            //Debug.Log("hit " + hit.transform.name + " under the player");
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;

            //������¼�⵽��������Ŀǰ��������״̬
            if (playerManager.isInAir)
            {
                //����ʱ���㹻��ʱ�����Ŵ�������ɵ��ص�����Ķ���
                if (inAirTimer > 0.5f)
                {
                    Debug.Log("You were in the air for " + inAirTimer);
                    //animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Empty", false);
                }
                inAirTimer = 0;
                playerManager.isInAir = false;
            }
        }
        else
        {
            //����û�м�⵽���壬���Ŀǰ���Ǵ���InGround״̬��Ҫ���ɵ�InAir״̬
            if (playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if (playerManager.isInAir == false)
            {
                if (playerManager.isInteracting == false)
                {
                    //animatorHandler.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
            }
        }

        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (playerManager.isInteracting) return;

        //moveDirection = cameraTransform.forward * inputHandler.vertical;
        //moveDirection += cameraTransform.right * inputHandler.horizontal;
        //moveDirection.Normalize();
        //moveDirection.y = 0;
        //Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
        //myTransform.rotation = jumpRotation;

        if (inputHandler.jump_Input)
        {
            animatorHandler.PlayTargetAnimation("Jump", true);
        }

        if (moveDirection.sqrMagnitude > 0)
        {
            rigidbody.AddForce(transform.forward.normalized * 10);
        }
    }
    #endregion
}