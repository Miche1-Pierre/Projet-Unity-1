using UnityEngine;

public class PlayerSwimming : MonoBehaviour
{
    private Rigidbody2D _rb2D;
    private Animator _animator;

    [Header("Water Settings")]
    public LayerMask waterLayer;
    public Transform waterCheck;
    public float waterCheckRadius = 0.05f; 
    public float swimSpeed = 1.8f; 
    public float buoyancyForce = 10f;
    public float surfaceLevelOffset = 0f;
    private bool _isInWater;
    private bool _isAtSurface;
    public float floatOscillationStrength = 6f;
    public float floatOscillationSpeed = 4f;

    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectWater();

        if (_isInWater)
        {
            HandleSwimming();
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (_isInWater)
        {
            ApplyBuoyancy();
            ApplyFloatAtSurface();
        }
    }

    private void DetectWater()
    {
        _isInWater = Physics2D.OverlapCircle(waterCheck.position, waterCheckRadius, waterLayer);

        if (_isInWater)
        {
            EnableSwimmingPhysics();
        }
        else
        {
            DisableSwimmingPhysics();
        }
    }

    private void HandleSwimming()
    {
        float moveX = Input.GetAxisRaw("Horizontal");     float moveY = Input.GetAxisRaw("Vertical");
        _rb2D.linearVelocity = new Vector2(moveX * swimSpeed, moveY * swimSpeed);
    }

    private void ApplyBuoyancy()
    {
        float depth = waterCheck.position.y - (transform.position.y - surfaceLevelOffset);
        if (depth > 0)
        {
            _rb2D.AddForce(Vector2.up * depth * buoyancyForce, ForceMode2D.Force);
        }

        _isAtSurface = depth < surfaceLevelOffset && Mathf.Abs(_rb2D.linearVelocityY) < 0.1f;
    }

    private void ApplyFloatAtSurface()
    {
        if (_isAtSurface)
        {
            float oscillation = Mathf.Sin(Time.time * floatOscillationSpeed) * floatOscillationStrength;
            _rb2D.AddForce(Vector2.up * oscillation, ForceMode2D.Force);
        }
    }

    private void EnableSwimmingPhysics()
    {
        _rb2D.gravityScale = 0f;
    }

    private void DisableSwimmingPhysics()
    {
        _rb2D.gravityScale = 1f;
    }

    private void UpdateAnimations()
    {
        if (_isInWater)
        {
            bool isMoving = Mathf.Abs(_rb2D.linearVelocityX) > 0.1f || Mathf.Abs(_rb2D.linearVelocityY) > 0.1f;

            _animator.SetBool("IsSwimming", isMoving);
        }
        else
        {
            _animator.SetBool("IsSwimming", false);
        }
    }

    private void OnDrawGizmos()
    {
        if (waterCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(waterCheck.position, waterCheckRadius);
        }
    }
}
