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
            new ObstacleType(1, 5, 8, 0.225f, 0.225f, 3, 10, 0.3f, smallObstacleSprites, () => GameState.instance.GetSettings().smallObstacleWeight),
            new ObstacleType(0.5f, 10, 10, 0.25f, 0.25f, 1, 2, 0.13f, mediumObstacleSprites, () => GameState.instance.GetSettings().mediumObstacleWeight),
            new ObstacleType(0.3f, 15, 15, 0.225f, 0.225f, 1, 1, 0.4f, bigObstacleSprites, () => GameState.instance.GetSettings().largeObstacleWeight),
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
        var polygonCollider2D = freeObstacle.GetComponent<PolygonCollider2D>();
        freeObstacle.transform.position = new Vector3(Random.Range(GameState.instance.GetRightWall().HighestCorner.x - obstacleType.CalculateWidth, GameState.instance.GetLeftWall().HighestCorner.x + obstacleType.CalculateWidth), GameState.instance.GetLeftWall().HighestCorner.y, 0);
        freeObstacle.GetComponent<SpriteRenderer>().sprite = obstacleType.Sprites[spriteIndex];
        Destroy(freeObstacle.GetComponent<PolygonCollider2D>());
        freeObstacle.AddComponent<PolygonCollider2D>();
        freeObstacle.GetComponent<Rigidbody2D>().mass = obstacleType.WeightGetter();
        freeObstacle.GetComponent<ObstacleScript>().speedModifier = obstacleType.SpeedModifier;
        freeObstacle.SetActive(true);
        PlayFlyingSound sound = freeObstacle.GetComponent<PlayFlyingSound>();
        if (sound == null)
        {
            sound = freeObstacle.AddComponent<PlayFlyingSound>();
        }
        else
        {
            sound.MakeDefault();
        }
        sound.SetStoneSound((int)obstacleType.MinDelay);
    }

    public void DisableObstcales()
    {
        foreach(GameObject obs in _spawnedObstacles)
        {
            obs.SetActive(false);
        }
    }
}

public class ObstacleType
{
    public float SpeedModifier { get; set; }
    public float MinDelay { get; set; }
    public float MaxDelay { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public float CalculateWidth { get; set; }
    public System.Func<float> WeightGetter { get; set; }
    public Sprite[] Sprites { get; set; }

    public ObstacleType(float speedModifier, float minDelay, float maxDelay, float width, float height, int minCount, int maxCount, float calculateWidth, Sprite[] sprites, System.Func<float> weightGetter)
    {
        SpeedModifier = speedModifier;
        MinDelay = minDelay;
        MaxDelay = maxDelay;
        Width = width;
        Height = height;
        MinCount = minCount;
        MaxCount = maxCount;
        Sprites = sprites;
        CalculateWidth = calculateWidth;
        WeightGetter = weightGetter;
    }
}
