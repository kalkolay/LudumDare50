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
            new ObstacleType(1, 5, 8, 0.225f, 0.225f, 3, 10, 0.3f, smallObstacleSprites, () => GameState.instance.GetSettings().smallObstacleWeight, StoneSize.small),
            new ObstacleType(0.6f, 10, 10, 0.25f, 0.25f, 1, 2, 0.13f, mediumObstacleSprites, () => GameState.instance.GetSettings().mediumObstacleWeight, StoneSize.medium),
            new ObstacleType(0.3f, 15, 15, 0.225f, 0.225f, 1, 1, 0.4f, bigObstacleSprites, () => GameState.instance.GetSettings().largeObstacleWeight, StoneSize.large),
        };
        for (int i = 0; i < 10; i++)
            _spawnedObstacles.Add(InstantiateObstacle());
        _delayModifier = 1f;
    }

    void Update()
    {
#if UNITY_EDITOR
        var isObstaclesEnabled = GameState.instance.GetSettings().isObstaclesEnabled;
#else
        var isObstaclesEnabled = true;
#endif
        if (Time.timeScale != 0 && isObstaclesEnabled && _spawnerCoroutines.Any( x => x is null))
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
#if UNITY_EDITOR
        var isObstaclesEnabled = GameState.instance.GetSettings().isObstaclesEnabled;
#else
        var isObstaclesEnabled = true;
#endif
        for (int i = 0; i < numberToSpawn; i++)
            StartCoroutine(SpawnObstacle(obstacleType));
        if (isObstaclesEnabled)
            _spawnerCoroutines[index] = StartCoroutine(SpawnObstaclesCoroutine(index));
        else
            _spawnerCoroutines[index] = null;
    }

    private IEnumerator SpawnObstacle(ObstacleType obstacleType)
    {
        yield return new WaitForSeconds(Random.Range(0, 3));
        var freeObstacle = _spawnedObstacles.FirstOrDefault(x => x.activeSelf == false);
        if (freeObstacle is null)
        {
            freeObstacle = InstantiateObstacle();
            _spawnedObstacles.Add(freeObstacle);
        }
        freeObstacle.transform.localScale = new Vector3(obstacleType.Width, obstacleType.Height, 1);
        var spriteIndex = Random.Range(0, obstacleType.Sprites.Length - 1);
        var polygonCollider2D = freeObstacle.GetComponent<PolygonCollider2D>();
        var spawnPositionX = Random.Range(GameState.instance.GetRightWall().HighestCorner.x - obstacleType.CalculateWidth, GameState.instance.GetLeftWall().HighestCorner.x + obstacleType.CalculateWidth);
        var spawnPositionY = Random.Range(-1, 1);
        freeObstacle.transform.position = new Vector3(spawnPositionX, GameState.instance.Sky.transform.position.y +2f + spawnPositionY, 0);
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
        if(obstacleType.Size == StoneSize.small)
        {
            sound.SetStoneSound((int)obstacleType.MinDelay, true);
        }
        else
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

public enum StoneSize
{
    small,
    medium,
    large
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
    public StoneSize Size { get; set; }

    public ObstacleType(float speedModifier, float minDelay, float maxDelay, float width, float height, int minCount, int maxCount, float calculateWidth, Sprite[] sprites, System.Func<float> weightGetter, StoneSize size)
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
        Size = size;
    }
}
