using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions, IDamageable
{
    public const float lookMaxAngleUp = -90f;
    public const float lookMaxAngleDown = 50f;

    public float lookSensitivity = 2.5f;

    public float walkingSpeed = 3.0f;
    public float runningSpeed = 5.0f;
    public float Health { get; private set; }

    PlayerInput input;
    CharacterController characterController;
    Animator animator;
    Camera camera;

    int isWalkingHash;
    int isRunningHash;

    Vector3 currentMovement;
    bool isMovementPressed;
    bool isRunPressed;

    float rotatePlayer = 0.0f;
    float rotateCamera = 0.0f;

    float damage = 50f;

    private void Awake()
    {
        // initially set reference variables
        input = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        PlayerInput.PlayerActions;
    }

    private void Start()
    {
        Camera[] _cameras = GetComponentsInChildren<Camera>();

        foreach (Camera cam in _cameras)
        {
            if (cam.name == "FirstPersonCamera")
            {
                //firstPersonCamera = cam;
                camera = cam;
            }
            /*
            else if (cam.name == "ThirdPersonCamera")
            {
                thirdPersonCamera = cam;
            }
            */
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 currentMovementInput;

        currentMovementInput = ctx.ReadValue<Vector2>();

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    public void OnLook(InputAction.CallbackContext ctx) { }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext ctx) { }

    public void OnInventory(InputAction.CallbackContext ctx) { }

    public void OnPickup(InputAction.CallbackContext ctx) { }

    public void OnInteract(InputAction.CallbackContext ctx) { }

    public void OnPrimaryAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0));
            RaycastHit hit;

            Physics.Raycast(ray, out hit, 10f);

            hit.collider.gameObject.SendMessage("OnDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnSecondaryAttack(InputAction.CallbackContext ctx) { }

    public void OnAim(InputAction.CallbackContext ctx) { }

    private void HandleRotation()
    {
        Vector2 lookDelta = input.Player.Look.ReadValue<Vector2>();

        rotatePlayer += lookDelta.x * lookSensitivity;
        rotateCamera = Mathf.Clamp(rotateCamera + (-lookDelta.y * lookSensitivity), lookMaxAngleUp, lookMaxAngleDown);

        transform.rotation = Quaternion.Euler(Vector3.up * rotatePlayer);
        camera.transform.rotation = Quaternion.Euler(new Vector3(rotateCamera, rotatePlayer));
    }

    private void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // start walking if isMovementPressed is true and not already walking
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        // stop walking if IsMovementPressed is false and we are already walking
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        // move camera
        //firstPersonCamera.transform.localPosition = new Vector3(0.0f, 1.636f, isRunPressed ? 0.493f : 0.293f);
    }

    private void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (characterController.isGrounded)
        {
            currentMovement.y = 0;
        }
        else
        {
            currentMovement.y -= 9.8f * Time.deltaTime;
        }
        
        Vector3 moveMotion = transform.TransformDirection((isRunPressed ? runningSpeed : walkingSpeed) * Time.deltaTime * currentMovement);
        characterController.Move(moveMotion);
    }

    public void OnDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }
}
