using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputDirection {
    Up,
    Down,
    Left,
    Right
}

public class PlayerInputControls : MonoBehaviour
{
    public PlayerInputActions playerContols;

    private float penaltyTimerMax, penaltyTimer;
    private bool isPenalized;

    private InputAction inputUp;
    private InputAction inputDown;
    private InputAction inputLeft;
    private InputAction inputRight;

    private void Awake() {
        playerContols = new PlayerInputActions();
    }

    private void OnEnable() {
        inputUp = playerContols.Player.InputUp;
        inputUp.Enable();
        inputUp.performed += UpInputPerformed;
        inputDown = playerContols.Player.InputDown;
        inputDown.Enable();
        inputDown.performed += DownInputPerformed;
        inputLeft = playerContols.Player.InputLeft;
        inputLeft.Enable();
        inputLeft.performed += LeftInputPerformed;
        inputRight = playerContols.Player.InputRight;
        inputRight.Enable();
        inputRight.performed += RightInputPerformed;
    }

    private void OnDisable() {
        inputUp.Disable();
        inputUp.performed -= UpInputPerformed;
        inputDown.Disable();
        inputDown.performed -= DownInputPerformed;
        inputLeft.Disable();
        inputLeft.performed -= LeftInputPerformed;
        inputRight.Disable();
        inputRight.performed -= RightInputPerformed;
    }

    // Start is called before the first frame update
    void Start()
    {
        penaltyTimerMax = 1.0f;
        penaltyTimer = penaltyTimerMax;
        isPenalized = false;
    }

    private void FixedUpdate() {
        if(isPenalized) {
            Debug.Log("Penalized!");
            penaltyTimer -= Time.deltaTime;
            if(penaltyTimer <= 0.0f) {
                Unpenalize();
            }
        }
    }

    public void Penalize() {
        penaltyTimer = penaltyTimerMax;
        isPenalized = true;
        playerContols.Disable();
    }

    private void Unpenalize() {
        isPenalized = false;
        playerContols.Enable();
    }

    private void UpInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(0, InputDirection.Up);
    }

    private void DownInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(0, InputDirection.Down);
    }

    private void LeftInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(0, InputDirection.Left);
    }

    private void RightInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(0, InputDirection.Right);
    }
}
