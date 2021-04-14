using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    
    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private Vector3 movement;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!isLocalPlayer) return;

        vertical = Input.GetKey(KeyCode.W) ? 1.0f : 
            Input.GetKey(KeyCode.S) ? -1.0f : 0;
        
        horizontal = Input.GetKey(KeyCode.D) ? 1.0f : 
            Input.GetKey(KeyCode.A) ? -1.0f : 0;
        
        movement = new Vector3(horizontal, 0, vertical);

        transform.position += movement * (moveSpeed * Time.deltaTime);
    }
}
