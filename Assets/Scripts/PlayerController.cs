using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action successfulJump;
    public static event Action playerDied;

    private Rigidbody2D playerRb;
    private CircleCollider2D circleCollider;

    private float colliderSizeDefault = 0.5f;
    private float colliderSizeInAir = 0.2f;

    private Vector3 circleSizeDefault = new Vector3(1, 1, 1);
    private Vector3 circleSizeSquashed = new Vector3(1.3f, 0.7f, 1);
    [SerializeField] private float circleSizeLerp = 0;
    private float circleSizeIncrement = 3f;

    private bool isOnWall;
    private bool gameOver = false;

    private int xForceToUse;
    [SerializeField] private float xForce = 45.0f;
    private float vertMovement = 0.02f;
    private float vertSpeed = 300.0f;
    private float killPlaneY = -6.0f;
    private float boundsY = 4.5f;

    public AudioSource hitSound;
    public AudioSource bounceSound;
    private float bounceVolume = 0.4f;

    public ParticleSystem dustCloud;

    void Start()
    {
        Wall.wallTooHot += PlayerBurned;

        // Randomize direction player jumps at start of game
        xForceToUse = UnityEngine.Random.Range(0, 2);
        float[] xForceValues = { xForce, -xForce };
        xForce = xForceValues[xForceToUse];
        Debug.Log("xForce is " + xForce);

        playerRb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        isOnWall = true;

        bounceSound = GetComponent<AudioSource>();
        bounceSound.volume = 0.0f;
    }

    void Update()
    {
        // Player jump from wall
        if (Input.GetKeyDown(KeyCode.Space) && isOnWall && !gameOver)
        {
            playerRb.AddForce(new Vector2(xForce, 0), ForceMode2D.Impulse);
            isOnWall = false;
            xForce = -xForce;

            circleCollider.radius = colliderSizeInAir;

            StartCoroutine(SquashAnimation());
        }

        // Player vertical movement along wall
        if (Input.GetKey(KeyCode.W) && isOnWall && !gameOver)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + vertMovement * vertSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) && isOnWall && !gameOver)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + -vertMovement * vertSpeed * Time.deltaTime);
        }

        // Deactivate player if they fall
        if (transform.position.y < killPlaneY)
        {
            gameObject.SetActive(false);
        }

        if (transform.position.y > boundsY && !gameOver)
        {
            transform.position = new Vector2(transform.position.x, boundsY);
        }

        if (transform.position.y < -boundsY && !gameOver)
        {
            transform.position = new Vector2(transform.position.x, -boundsY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopAllCoroutines();
            isOnWall = true;
            circleCollider.radius = colliderSizeDefault;
            circleSizeLerp = 0;
            transform.localScale = circleSizeDefault;

            if (!gameOver)
            {
                bounceSound.volume = bounceVolume;
                bounceSound.Play();
                successfulJump?.Invoke();
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            hitSound.Play();
            dustCloud.Play();
            gameOver = true;
            StopAllCoroutines();
            circleCollider.radius = colliderSizeDefault;
            playerRb.gravityScale = 2.0f;
            circleSizeLerp = 0;
            transform.localScale = circleSizeDefault;

            playerDied?.Invoke();
        }
    }

    IEnumerator SquashAnimation()
    {
        while (!isOnWall)
        {
            circleSizeLerp += circleSizeIncrement * Time.deltaTime;

            transform.localScale = Vector3.Lerp(circleSizeDefault, circleSizeSquashed, circleSizeLerp);
            yield return null;
        }
    }

    private void PlayerBurned()
    {
        playerRb.AddForce(new Vector2(xForce / 10, 20.0f), ForceMode2D.Impulse);
        gameOver = true;
        StopAllCoroutines();
        circleCollider.radius = colliderSizeDefault;
        playerRb.gravityScale = 2.0f;
        circleSizeLerp = 0;
        transform.localScale = circleSizeDefault;

        playerDied?.Invoke();
    }
}
