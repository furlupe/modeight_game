using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    public int Health { get; set; }
    public int Damage { get; set; }
    public Animator Animator;
    public Rigidbody2D rb;

    public AudioSource Audio;
    public GameObject Player;
    public Vector2 FieldOfView { get; set; }

    public AudioClip hitSound;
    public AudioClip deathSound;

    public readonly int _hurt = Animator.StringToHash("Hurt");
    public readonly int _isDead = Animator.StringToHash("isDead");
    protected readonly int PlayerSpotted = Animator.StringToHash("playerSpotted");

    [SerializeField] private LayerMask playerMask;

    public Vector2 outsideForces = Vector2.zero;
    public int dir;
    public bool facingRight;
    private Vector2 knockBackForce = new(3, 0);
    public float knockBackDuration = 1f;


    protected void Init(int h, int dmg)
    {
        Animator = GetComponent<Animator>();
        Audio = transform.GetChild(0).GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");

        Health = h;
        Damage = dmg;
        dir = -1;
        facingRight = false;
    }

    private void Update()
    {
        outsideForces = Vector2.Lerp(outsideForces, Vector2.zero, knockBackDuration * Time.deltaTime);
    }

    public void Disable()
    {
        enabled = false;
    }

    public void TakeDamage(int incomeDamage)
    {
        Health -= incomeDamage;
        Animator.SetTrigger(_hurt);
        Audio.PlayOneShot(hitSound);

        if (Health <= 0)
        {
            Die();
            return;
        }

        KnockBack();
    }

    public virtual void Die()
    {
        
    }

    public void KnockBack()
    {
        Animator.SetBool(PlayerSpotted, false);
        rb.velocity = Vector2.zero;
        AddForce(knockBackForce);
    }

    private void AddForce(Vector2 force)
    {
        outsideForces += force * (-dir);
    }

    public void Flip()
    {
        dir *= -1;
        facingRight = !facingRight;

        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool CheckIfPlayerWithinFov(Vector2 fov)
    {
        for (var i = 0; i < 361; i += 15)
        {
            var radian = i * Mathf.Deg2Rad;

            var direction = new Vector2(
                Vector2.left.x * Mathf.Cos(radian) - Vector2.left.y * Mathf.Sin(radian),
                Vector2.left.x * Mathf.Sin(radian) + Vector2.left.y * Mathf.Cos(radian)
            );

            var hit = Physics2D.Raycast(transform.position, direction, fov.x, playerMask);

            //Debug.DrawRay(transform.position, direction, Color.green);
            if (hit.collider != null)
                return true;
        }

        return false;
    }
}