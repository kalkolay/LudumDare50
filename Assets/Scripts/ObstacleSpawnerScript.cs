using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleSpawnerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject obstacle;

    private List<GameObject> _spawnedObstacles = new List<GameObject>();
    private float _delayModifier;

    void Awake()
    {
        for (int i = 0; i < 2; i++)
            _spawnedObstacles.Add(InstantiateObstacle());
        _delayModifier = 1f;
        StartCoroutine(SpawnObstacleCoroutine());
    }

    void Update()
    {
        
    }

    private GameObject InstantiateObstacle()
    {
        var result = Instantiate(obstacle, transform);
        result.SetActive(false);
        return result;
    }

    public void SpeedUp()
    {
        _delayModifier *= GameState.instance.GetSettings().obstaclesDelayModifier;
    }

    private IEnumerator SpawnObstacleCoroutine()
    {
        if (!GameState.instance.GetSettings().isObstaclesEnabled)
            yield break;
        yield return new WaitForSeconds(GameState.instance.GetSettings().obstaclesInitSpawnDelay * _delayModifier);
        var freeObstacle = _spawnedObstacles.FirstOrDefault(x => x.activeSelf == false);
        if (freeObstacle is null)
        {
            freeObstacle = InstantiateObstacle();
            _spawnedObstacles.Add(freeObstacle);
        }
        var width = freeObstacle.GetComponent<BoxCollider2D>().size.x * freeObstacle.transform.localScale.x / 2;
        freeObstacle.transform.position = new Vector3(Random.Range(GameState.instance.GetRightWall().HighestCorner - width, GameState.instance.GetLeftWall().HighestCorner + width), 5.45f, 0);
        freeObstacle.SetActive(true);
        StartCoroutine(SpawnObstacleCoroutine());
    }
}
