using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawnerScript : MonoBehaviour
{
    public GameObject pipePrefab;
    public float initialSpawnRate = 3f;
    public float minSpawnRate = 0.5f;
    public float spawnRateDecreaseFactor = 5f;
    public float heightOffsetPercentage = 0.3f;
    private float _currentSpawnRate;
    private float _timer = 0f;

    private List<GameObject> _pipePool;
    public int poolSize = 5;
    public float pipeSpeed = 5f;
    private float _screenRightEdge;

    void Start()
    {
        _currentSpawnRate = initialSpawnRate;

        // Calculate the right edge of the screen based on 1080p resolution
        _screenRightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2f;

        // Initialize the pipe object pool
        InitializePipePool();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        // Spawn pipes based on current spawn rate
        if (_timer >= _currentSpawnRate)
        {
            SpawnPipeFromPool();
            AdjustSpawnRate();
            _timer = 0f;
        }

        // Move pipes and recycle them
        UpdatePipeMovement();
    }

    void InitializePipePool()
    {
        _pipePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pipe = Instantiate(pipePrefab);
            pipe.SetActive(false);  // Disable initially
            _pipePool.Add(pipe);
        }
    }

    void SpawnPipeFromPool()
    {
        foreach (GameObject pipe in _pipePool)
        {
            if (!pipe.activeInHierarchy)
            {
                float lowestPoint = transform.position.y - (Screen.height * heightOffsetPercentage) / 100f;
                float highestPoint = transform.position.y + (Screen.height * heightOffsetPercentage) / 100f;
                float randomHeight = Random.Range(lowestPoint, highestPoint);

                // Set pipe position offscreen to the right
                pipe.transform.position = new Vector3(_screenRightEdge, randomHeight, 0);
                pipe.SetActive(true);
                break;
            }
        }
    }

    void AdjustSpawnRate()
    {
        // Decrease spawn rate over time for increased difficulty
        if (_currentSpawnRate > minSpawnRate)
        {
            _currentSpawnRate -= spawnRateDecreaseFactor;
        }
    }

    void UpdatePipeMovement()
    {
        foreach (GameObject pipe in _pipePool)
        {
            if (pipe.activeInHierarchy)
            {
                // Move the pipe to the left
                pipe.transform.Translate(Vector3.left * pipeSpeed * Time.deltaTime);

                // Deactivate the pipe once it moves off the screen
                if (pipe.transform.position.x < -_screenRightEdge)
                {
                    pipe.SetActive(false);
                }
            }
        }
    }
}
