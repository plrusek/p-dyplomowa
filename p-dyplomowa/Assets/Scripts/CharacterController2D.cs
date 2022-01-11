using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_Jumped;
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody m_Rigidbody;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private bool m_wasCrouching = false;

    public Actions actions;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded && !m_Jumped)
                {
                    actions.Land();
                }
            }
        }
        m_Jumped = false;
    }


    public void Move(float move, bool crouch, bool jump)
    {
        var movementSmoothing = m_MovementSmoothing;
        if (!m_Grounded && m_AirControl)
        {
            actions.Jump();
            movementSmoothing = m_MovementSmoothing * 3f;
        }
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }
        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                }
                move *= m_CrouchSpeed;
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                }
            }
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody.velocity.y);
            m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
            var xspeed = Math.Abs(m_Rigidbody.velocity.x);
            actions.Move(xspeed);
            if (m_Grounded)
            {
                if (move > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight)
                {
                    Flip();
                }
            }
            if (m_FacingRight)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), 0.2f);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 270, 0), 0.2f);
        }
        if (m_Grounded && jump)
        {
            m_Jumped = true;
            m_Grounded = false;
            m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
            actions.Jump();
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Quaternion theRotation = transform.rotation;
    }
}