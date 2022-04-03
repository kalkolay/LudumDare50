using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField]
    private SettingsSO settingsSo;
    [SerializeField]
    private GameObject spawner;
    [SerializeField]
    private WallScript leftWall;
    [SerializeField]
    private WallScript rightWall;

    [System.NonSerialized]
    public static GameState instance = null;

    private ObstacleSpawnerScript _spawner;

    void Awake()
    {
        instance = this;
        var spawnerGO = Instantiate(spawner, new Vector3(0, 0, 1), Quaternion.identity);
        _spawner = spawnerGO.GetComponent<ObstacleSpawnerScript>();
    }

    void Update()
    {
        
    }

    public SettingsSO GetSettings()
    {
        return settingsSo;
    }

    public WallScript GetLeftWall()
    {
        return leftWall;
    }

    public WallScript GetRightWall()
    {
        return rightWall;
    }

    public void SpeedUp()
    {
        _spawner.SpeedUp();
    }

    public void MoveWalls(float distance)
    {
        leftWall.MoveWall(distance);
        rightWall.MoveWall(distance);
    }
}
