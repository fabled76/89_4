using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadAir
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        private Rigidbody2D body;
        private InputAction move;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            move = new InputAction("Walk", InputActionType.Value);
            move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
            move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
        }

        private void OnEnable() => move.Enable();
        private void FixedUpdate()
        {
            // Preserve vertical physics; walking never teleports through a wall.
            body.linearVelocity = new Vector2(move.ReadValue<float>() * moveSpeed, body.linearVelocity.y);
        }

        private void OnDisable()
        {
            move.Disable();
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        }

        private void OnDestroy() => move.Dispose();
    }
}
