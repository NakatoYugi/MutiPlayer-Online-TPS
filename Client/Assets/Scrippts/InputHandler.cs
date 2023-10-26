using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool jump_Input;
    public bool attack_Input;

    PlayerActions inputActions;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void OnEnable()
    {
        if (inputActions == null) 
        {
            inputActions = new PlayerActions();
        }

        inputActions.Enable();
        inputActions.Gameplay.Jump.performed += i => jump_Input = true;
        inputActions.Gameplay.Jump.canceled += i => jump_Input = false;
        inputActions.Gameplay.Attack.performed += i => attack_Input = true;
        inputActions.Gameplay.Attack.canceled += i => attack_Input = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    private void Update()
    {
        movementInput = inputActions.Gameplay.Move.ReadValue<Vector2>();
        cameraInput = inputActions.Gameplay.Look.ReadValue<Vector2>();
        float delta = Time.deltaTime;
        TickInput(delta);
    }

    private void LateUpdate()
    {
        
    }

    private void TickInput(float deltaTime)
    {
        MoveInput(deltaTime);
    }

    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }
}
