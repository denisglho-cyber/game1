using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }    
    private PlayerInputActions playerInputActions;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerDash;
    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.combat.attack.started += Attack_started;

        playerInputActions.Player.Dash.performed += PlayerDash_perfomed;
    }


    private void PlayerDash_perfomed(InputAction.CallbackContext obj)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.move.ReadValue<Vector2>();

        return inputVector;
    }
    public Vector3 GetMousePosition()
    {
        Vector3 mousePose = Mouse.current.position.ReadValue();
    return mousePose;
    }

    internal Vector3 GetMousPosition()
    {
        throw new NotImplementedException();
    }
}
