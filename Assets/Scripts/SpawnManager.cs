using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstacles;
    private int obstacleIndex = 0;
    private bool gameStarted = false;

    private void Start()
    {
        Obstacle.obstacleDisabled += GenerateRandomIndex;
        Obstacle.obstacleDisabled += ActivateObstacle;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        {
            GenerateRandomIndex();
            ActivateObstacle();
            gameStarted = true;
        }
    }

    private void GenerateRandomIndex()
    {
        obstacleIndex = Random.Range(0, obstacles.Length);
    }

    private void ActivateObstacle()
    {
        obstacles[obstacleIndex].SetActive(true);
    }
}
