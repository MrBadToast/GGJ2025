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

    [Title("")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private AudioSource audio;
    [SerializeField] private Rigidbody2D rBody;
    public Rigidbody2D RigBody { get { return rBody; } }
    [SerializeField] private GameObject hitCamera;

    [Title("")]
    [SerializeField] private Transform RfootPosition;
    [SerializeField] private Transform LfootPosition;
    [SerializeField] private float footDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AnimationCurve fallCurve;
    public bool controlEnabled = false;

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

        audio = GetComponent<AudioSource>();
        rBody = GetComponent<Rigidbody2D>();

    }

    [SerializeField,ReadOnly] private bool grounded = false;
    private float jumpKeep = 0.2f;
    private float jumpTimer = 0f;

    private void Update()
    {
        grounded = CheckGrounded();
        animator.SetBool("Grounded", false);

        if (grounded)
        {
            if (rBody.velocity.y < 0.01f)
            {
                animator.SetBool("Grounded", true);
                jumpTimer = jumpKeep;
            }
        }

        jumpTimer -= Time.deltaTime;
        

        if(controlEnabled)
        {
            animator.SetBool("Move", false);

            if (Input.GetKey(RmoveKey))
            {
                rBody.velocity = new Vector2(moveSpeed, rBody.velocity.y);
                spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                animator.SetBool("Move", true);

            }
            if(Input.GetKey(LmoveKey))
            {
                rBody.velocity = new Vector2(-moveSpeed, rBody.velocity.y);
                spriteTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                animator.SetBool("Move", true);
            }

            if(!Input.GetKey(RmoveKey) && !Input.GetKey(LmoveKey))
            {
                rBody.velocity = new Vector2(Mathf.Lerp(rBody.velocity.x, 0f, horizontalDrag), rBody.velocity.y);
            }

            if(Input.GetKey(jumpKey))
            {
                if (jumpTimer >= 0f)
                {
                    rBody.velocity = new Vector2(rBody.velocity.x, jumpPower);
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

    public void OnPlayerHarmed(bool ignoreCamera = false)
    {
        if (!controlEnabled) return;
        controlEnabled = false;
        GameplaySystem.Instance.OnPlayerHarmed();
        StopAllCoroutines();
        StartCoroutine(Cor_PlayerHarmed(ignoreCamera));
    }

    public IEnumerator Cor_PlayerHarmed(bool ignoreCamera)
    {
        if (!ignoreCamera)
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.1f * 0.02f;
            hitCamera.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
            hitCamera.SetActive(false);
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
        }

        rBody.velocity = Vector2.zero;
        rBody.isKinematic = true;

        float start = transform.position.y;
        float dest = GameplaySystem.Instance.BubblePosition.y - 10f;

        for(float time = 0f; time < 2f; time += Time.deltaTime)
        {
            float t = time / 2f;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(start,dest,t) + fallCurve.Evaluate(t)*10f, 0f);
            transform.Rotate(new Vector3(0f, 0f, 2f));
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            OnPlayerHarmed();
        }
        if (collision.gameObject.layer == 11)
        {
            OnPlayerHarmed(true);
        }
        if (collision.gameObject.layer == 12)
        {
            rBody.velocity = new Vector2(rBody.velocity.x, jumpPower*2.0f);
        }
        if(collision.gameObject.layer == 13)
        {
            Bubble.Instance.ActivateBarrier();
            Destroy(collision.gameObject);
        }


        if(collision.gameObject.layer == 10)
        {
            collision.GetComponent<NightmareInstance>().OnPlayerHitted();
        }    
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
