using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public static event Action wallTooHot;

    private SpriteRenderer spriteRenderer;
    public Color wallColorCool;
    public Color wallColorHot;

    private Coroutine lastCoroutine;

    [SerializeField] private float lerpAmount = 0.0f;
    private float lerpIncrease = 0.2f;
    private float lerpDecrease = 0.2f;
    private float lerpSpeed = 1.0f;

    public ParticleSystem dustCloud;
    public float dustOffsetZ;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerController.playerDied += SetGameOver;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !gameOver)
        {
            dustCloud.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, dustOffsetZ);
            dustCloud.Play();

            if (lastCoroutine != null)
            {
                StopCoroutine(lastCoroutine);
            }

            lastCoroutine = StartCoroutine(WallHeatUp());
        }
    }

    IEnumerator WallHeatUp()
    {
        while (lerpAmount < 1)
        {
            lerpAmount += lerpIncrease * Time.deltaTime * lerpSpeed;
            if (lerpAmount > 1)
            {
                lerpAmount = 1;
                wallTooHot?.Invoke();
            }
            spriteRenderer.color = Color.Lerp(wallColorCool, wallColorHot, lerpAmount);
            yield return null;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            {
                if (lastCoroutine != null)
                {
                    StopCoroutine(lastCoroutine);
                }

                lastCoroutine = StartCoroutine(WallCoolDown());
            }
        }
    }

    IEnumerator WallCoolDown()
    {
        while (lerpAmount > 0)
        {
            lerpAmount -= lerpDecrease * Time.deltaTime * lerpSpeed;
            if (lerpAmount < 0)
            {
                lerpAmount = 0;
            }
            spriteRenderer.color = Color.Lerp(wallColorCool, wallColorHot, lerpAmount);
            yield return null;
        }
    }

    void SetGameOver()
    {
        gameOver = true;
    }
}
