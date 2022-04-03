using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleSpawnerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject obstacle;
    [SerializeField]
    private Sprite[] smallObstacleSprites;
    [SerializeField]
    private Sprite[] mediumObstacleSprites;
    [SerializeField]
    private Sprite[] bigObstacleSprites;

    private List<GameObject> _spawnedObstacles = new List<GameObject>();
    private float _delayModifier;
    private Coroutine[] _spawnerCoroutines = new Coroutine[3];

    private ObstacleType[] ObstaclesDescription;

    void Awake()
    {
        ObstaclesDescription = new ObstacleType[]
        {
            new ObstacleType(2, 5, 0.225f, 0.225f, 1, 5, smallObstacleSprites),
            new ObstacleType(5, 5, 0.25f, 0.25f, 1, 2, mediumObstacleSprites),
            new ObstacleType(10, 10, 0.225f, 0.225f, 1, 1, bigObstacleSprites),
        };
        for (int i = 0; i < 10; i++)
            _spawnedObstacles.Add(InstantiateObstacle());
        _delayModifier = 1f;
    }

    void Update()
    {
        if (Time.timeScale != 0 && GameState.instance.GetSettings().isObstaclesEnabled && _spawnerCoroutines.Any( x => x is null))
            for (var i = 0; i < ObstaclesDescription.Length; i++)
                _spawnerCoroutines[i] = StartCoroutine(SpawnObstaclesCoroutine(i));
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

    private IEnumerator SpawnObstaclesCoroutine(int index)
    {
        var obstacleType = ObstaclesDescription[index];
        var timeToWait = Random.Range(obstacleType.MinDelay, obstacleType.MaxDelay);
        var numberToSpawn = Random.Range(obstacleType.MinCount, obstacleType.MaxCount);
        yield return new WaitForSeconds(timeToWait * _delayModifier);
        for (int i = 0; i < numberToSpawn; i++)
            SpawnObstacle(obstacleType);
        if (GameState.instance.GetSettings().isObstaclesEnabled)
            _spawnerCoroutines[index] = StartCoroutine(SpawnObstaclesCoroutine(index));
        else
            _spawnerCoroutines[index] = null;
    }

    private void SpawnObstacle(ObstacleType obstacleType)
    {
        var freeObstacle = _spawnedObstacles.FirstOrDefault(x => x.activeSelf == false);
        if (freeObstacle is null)
        {
            freeObstacle = InstantiateObstacle();
            _spawnedObstacles.Add(freeObstacle);
        }
        freeObstacle.transform.localScale = new Vector3(obstacleType.Width, obstacleType.Height, 1);
        var spriteIndex = Random.Range(0, obstacleType.Sprites.Length - 1);
        var boxcollider = freeObstacle.GetComponent<BoxCollider2D>();
        boxcollider.size = obstacleType.Sprites[spriteIndex].bounds.size;
        var width = boxcollider.size.x * freeObstacle.transform.localScale.x / 2;
        freeObstacle.transform.position = new Vector3(Random.Range(GameState.instance.GetRightWall().HighestCorner - width, GameState.instance.GetLeftWall().HighestCorner + width), 5.45f, 0);
        freeObstacle.GetComponent<SpriteRenderer>().sprite = obstacleType.Sprites[spriteIndex];
        freeObstacle.SetActive(true);
        freeObstacle.AddComponent<PlayFlyingSound>();
    }
}

public class ObstacleType
{
    public float MinDelay { get; set; }
    public float MaxDelay { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public Sprite[] Sprites { get; set; }

    public ObstacleType(float minDelay, float maxDelay, float width, float height, int minCount, int maxCount, Sprite[] sprites)
    {
        MinDelay = minDelay;
        MaxDelay = maxDelay;
        Width = width;
        Height = height;
        MinCount = minCount;
        MaxCount = maxCount;
        Sprites = sprites;
    }
}
