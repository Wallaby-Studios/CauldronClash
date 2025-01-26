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

public class PlayerInputControls : EntityInput {
    private InputActionAsset inputAsset;
    private InputActionMap player;

    private InputAction inputUp;
    private InputAction inputDown;
    private InputAction inputLeft;
    private InputAction inputRight;

    private void Awake() {
		inputAsset = GetComponent<PlayerInput>().actions;
		player = inputAsset.FindActionMap("Player");
	}

    private void OnEnable() {
        player.FindAction("InputUp").started += UpInputPerformed;
		player.FindAction("InputDown").started += DownInputPerformed;
		player.FindAction("InputLeft").started += LeftInputPerformed;
		player.FindAction("InputRight").started += RightInputPerformed;
		player.Enable();
	}

    private void OnDisable() {
		player.FindAction("InputUp").started -= UpInputPerformed;
		player.FindAction("InputDown").started -= DownInputPerformed;
		player.FindAction("InputLeft").started -= LeftInputPerformed;
		player.FindAction("InputRight").started -= RightInputPerformed;
		player.Disable();
	}

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

    private void UpInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(index, InputDirection.Up);
    }

    private void DownInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(index, InputDirection.Down);
    }

    private void LeftInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(index, InputDirection.Left);
    }

    private void RightInputPerformed(InputAction.CallbackContext context) {
        GameManager.instance.CheckInput(index, InputDirection.Right);
    }
}
