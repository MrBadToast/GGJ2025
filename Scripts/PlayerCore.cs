using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCore : MonoBehaviour
{
    static private PlayerCore _instance;
    static public PlayerCore Instance { get { return _instance; } }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField,Range(0f,1f)] private float horizontalDrag = 0.5f;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode RmoveKey;
    [SerializeField] private KeyCode LmoveKey;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audio;
    [SerializeField] private Rigidbody2D rBody;

    [SerializeField] private Transform RfootPosition;
    [SerializeField] private Transform LfootPosition;
    [SerializeField] private float footDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField,ReadOnly] private bool controlEnabled = true;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance.gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        rBody = GetComponent<Rigidbody2D>();

    }


    [SerializeField,ReadOnly] private bool grounded = false;

    private float coyoteThreshold = 0.1f;
    private float coyoteTimer = 0f;

    private void Update()
    {
        grounded = CheckGrounded();

        if (grounded) coyoteTimer = coyoteThreshold;

        if (!grounded && coyoteTimer >= 0f)
        {
            coyoteTimer -= Time.deltaTime;
        }

        if(controlEnabled)
        {
            if(Input.GetKey(RmoveKey))
            {
                rBody.velocity = new Vector2(moveSpeed, rBody.velocity.y);
            }
            if(Input.GetKey(LmoveKey))
            {
                rBody.velocity = new Vector2(-moveSpeed, rBody.velocity.y);
            }

            if(!Input.GetKey(RmoveKey) && !Input.GetKey(LmoveKey))
            {
                rBody.velocity = new Vector2(Mathf.Lerp(rBody.velocity.x, 0f, horizontalDrag), rBody.velocity.y);
            }

            if(Input.GetKeyDown(jumpKey))
            {
                if (grounded)
                {
                    ExecuteJump();
                }
                else
                {
                    if (coyoteTimer >= 0f)
                    {
                        ExecuteJump();
                        coyoteTimer = -1f;
                    }
                }
            }
        }
    }

    private bool CheckGrounded()
    {
        var Rfoot = Physics2D.Raycast(RfootPosition.position, Vector2.down, footDistance, groundLayer.value);
        var Lfoot = Physics2D.Raycast(LfootPosition.position, Vector2.down, footDistance, groundLayer.value);

        if(Rfoot || Lfoot)
        {
            return true;
        }
        else return false;
    }

    private void ExecuteJump()
    {
        rBody.velocity = new Vector2(rBody.velocity.x, jumpPower);
    }

    private void OnDrawGizmos()
    {
        if (grounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(RfootPosition.position, RfootPosition.position + Vector3.down * footDistance);
        Gizmos.DrawLine(LfootPosition.position, LfootPosition.position + Vector3.down * footDistance);
    }
}
