using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        Vector3 movement = Vector3.zero;
        
        if (Keyboard.current.wKey.isPressed)
        {
            movement += Vector3.forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            movement += Vector3.back;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            movement += Vector3.left;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            movement += Vector3.right;
        }
        
        if (movement.magnitude > 0)
        {
            movement.Normalize();
        }
        
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Mouse aiming
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0; // Keep it flat on the ground
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}