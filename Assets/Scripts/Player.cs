using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerAction.IPlayerActions
{
    [SerializeField] private float speed = 1f;

    private Vector2 _direction;
    private Rigidbody _rb;
    private GameObject _interactable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!DialogData.InDialog)
        {
            _rb.linearVelocity = new Vector3(_direction.x, 0, _direction.y) * speed;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable _)
            && (
                _interactable == null ||
                Vector3.Distance(transform.position, other.gameObject.transform.position) <
                Vector3.Distance(transform.position, _interactable.transform.position)))
        {
            _interactable = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable _) && _interactable == other.gameObject)
        {
            _interactable = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && _interactable && !DialogData.InDialog)
        {
            _interactable.GetComponent<IInteractable>().Interact();
        }
    }
}