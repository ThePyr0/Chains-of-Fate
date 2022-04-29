using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //public PlayerInput playerInput;
    public CoFPlayerControls controls;
    public float speed = 10f;

    public event Action OnReady;

    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    
    private CombatUI CUI;
    private Vector2 move;
    private Rigidbody2D rb;
    private Champion player;
    
    private void Awake()
    {
        controls = new CoFPlayerControls();
        controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>(); //on button press gets the movement value and starts the movement
        controls.Player.Movement.canceled += ctx => move = Vector2.zero; //on button release stops movement
        //DontDestroyOnLoad(this);

        rb = GetComponent<Rigidbody2D>();
    }
 
    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        OnReady?.Invoke();

        player = ChainsOfFate.Gerallt.GameManager.Instance.GetPlayer();
    }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(move.x, move.y) * speed * player.MovementSpeed * Time.fixedDeltaTime; //gets a value based on actual seconds not pc specs that is used to calculate movement

        if (movement != Vector2.zero)
        {
            if (movement.x < 0)
            {
                characterSpriteRenderer.flipX = true;
            }
            else
            {
                characterSpriteRenderer.flipX = false;
            }
            
            //transform.Translate(movement,Space.World);
        
            // Switched to a rigidbody version instead of directly affecting transform
            // because there's a 2D collision system using 2D Colliders
            rb.MovePosition(rb.position + movement);
        }
    }
}
