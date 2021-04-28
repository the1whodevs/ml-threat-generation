using System;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController Instance { get; private set; }

    public enum PlayerMoveState { Idle, Moving, Stunned }
    public enum PlayerAttackState { Idle, Casting }

    public PlayerMoveState MoveState { get; private set; } = PlayerMoveState.Idle;
    public PlayerAttackState AttackState { get; private set; } = PlayerAttackState.Idle;
    
    [Header("Managers")]
    [SerializeField] private new PlayerAnimation animation;
    [SerializeField] private new PlayerCamera camera;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerAbilities abilities;
    [SerializeField] private PlayerCharacter character;

    [Header("Move Controls")] 
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backwardKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    [Header("Attack/Ability Controls")] 
    [SerializeField] private KeyCode abilityA_Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode abilityB_Key = KeyCode.Alpha2;
    
    private float stunTimer = 0.0f;
    private float castTimer = 0.0f;

    private float vertical = 0.0f;
    private float horizontal = 0.0f;
    
    private void Start()
    {
        if (isLocalPlayer)
        {
            Instance = this;
            camera.gameObject.SetActive(true);
            character.OnCharacterChanged += OnCharacterChanged;
        }
        else camera.gameObject.SetActive(false);    
    }

    private void OnDestroy()
    {
        character.OnCharacterChanged -= OnCharacterChanged;
    }

    private void OnCharacterChanged(PlayerCharacter.Character obj)
    {
        movement.SetMovementRotator(obj.movementRotator);
        animation.SetAnimator(obj.animator);
    }

    private void Update()
    {
        // If this is not the local player, don't check anything.
        if (!isLocalPlayer) return;

        // If no keys are pressed, don't check anything.
        if (!Input.anyKey)
        {
            MoveState = PlayerMoveState.Idle;
            animation.SetMoving(false);
            if (character.SelectedCharacter.useMovementRotations) movement.ResetRotation();
            return;
        }

        if (MoveState == PlayerMoveState.Stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0.0f) MoveState = PlayerMoveState.Idle;
        }
        
        // TODO: Check for cast time etc...
        if (Input.GetKeyDown(abilityA_Key))
        {
            abilities.AttackA();
            animation.SetAttackA();
        }
        else if (Input.GetKeyDown(abilityB_Key))
        {
            abilities.AttackB();
            animation.SetAttackB();
        }

        // Don't check for movement input if stunned.
        if (MoveState == PlayerMoveState.Stunned) return;
        
        // Player is not stunned and can move.

        if (Input.GetKey(forwardKey)) vertical = 1.0f;
        else if (Input.GetKey(backwardKey)) vertical = -1.0f;
        else vertical = 0;
        
        if (Input.GetKey(rightKey)) horizontal = 1.0f;
        else if (Input.GetKey(leftKey)) horizontal = -1.0f;
        else horizontal = 0;
        
        // If the player is moving...
        if (movement.HandleMovement(vertical, horizontal, character.SelectedCharacter.useMovementRotations))
        {
            animation.SetMoving(true);
            MoveState = PlayerMoveState.Moving;
            AttackState = PlayerAttackState.Idle;
        }
        else
        {
            animation.SetMoving(false);
            MoveState = PlayerMoveState.Idle;        
        }
    }

    /// <summary>
    /// Stuns the player for given time. When called while already stunned,
    /// it updates the stun timer to the new time, even if it's less than the current time.
    /// </summary>
    /// <param name="time"></param>
    public void Stun(float time)
    {
        stunTimer = time;

        MoveState = PlayerMoveState.Stunned;
        AttackState = PlayerAttackState.Idle;

        castTimer = 0;
    }

    /// <summary>
    /// Called by PlayerAbilities (?) when an ability starts casting.
    /// </summary>
    /// <param name="duration"></param>
    public void StartCast(float duration)
    {
        AttackState = PlayerAttackState.Casting;
    }

    /// <summary>
    /// Called by PlayerAbilities (?) while an ability is being cast.
    /// </summary>
    public void CastTick(float tick)
    {
        castTimer -= tick;

        if (castTimer > 0.0f) return;

        castTimer = 0;
        AttackState = PlayerAttackState.Idle;
    }
}
