using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static event Action obstacleDisabled;
    private static float speedCurrent = 3.5f;
    private float speedIncrement = 0.05f;
    public float yPosStart;

    private bool isActive;

    private void Start()
    {
        transform.position = new Vector2(0, yPosStart);
        PlayerController.successfulJump += IncreaseSpeed;
    }

    private void OnEnable()
    {
        isActive = true;
    }
    void Update()
    {
        if (isActive)
        {
            transform.Translate(Vector2.up * speedCurrent * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
        Invoke("ResetAndDisable", 0.1f);
        gameObject.SetActive(false);
    }

    private void ResetAndDisable()
    {
        transform.position = new Vector2(0, yPosStart);
        obstacleDisabled?.Invoke();
    }

    private void IncreaseSpeed()
    {
        speedCurrent += speedIncrement;
    }
}
