using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerAction.IPlayerActions
{
    public DialogData startDialog;
    
    [SerializeField] 
    private float speed = 1f;

    private Vector2 _direction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(startDialog.Play());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = _direction * (speed * Time.deltaTime);
        transform.Translate(movement.x, 0, movement.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact");
        }
    }
}
