using UnityEngine;
using System.Collections;

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
    [SerializeField]
    private BackgroundScript Background;
    public GameObject Sky;
    [SerializeField]
    private GameObject Floor;
    [SerializeField]
    private GameObject Ded;
    [SerializeField]
    private GameObject Light;

    [System.NonSerialized]
    public static GameState instance = null;
    [System.NonSerialized]
    public int currentHeight = 1;

    private ObstacleSpawnerScript _spawner;
    private float _totalMovedDistance = 0;

    public GameObject DedPrefab;
    public Menu menu;

    public int Score;
    private float floorPosZ;
    private float skyPosZ;
    public bool IsWin = false;
    private float SkyHeight = 8;
    private bool isSkiesShown = false;

    void Awake()
    {
        instance = this;
        var spawnerGO = Instantiate(spawner, new Vector3(0, 0, 1), Quaternion.identity, transform);
        _spawner = spawnerGO.GetComponent<ObstacleSpawnerScript>();
        Sky.GetComponent<DeathPlate>().OnSpawnDed += SpawnDed;
        Sky.SetActive(false);
        floorPosZ = Floor.transform.position.y;
        skyPosZ = Sky.transform.position.y;
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

    public bool OnCameraMove(float distance)
    {
        Background.Move(distance);
        if (_totalMovedDistance >= settingsSo.maxHeight)
        {
            Sky.SetActive(true);
            return false;
        }
        if (_totalMovedDistance >= settingsSo.maxHeight - SkyHeight - 2 && !isSkiesShown)
        {
            Instantiate(Light, new Vector3(0, settingsSo.maxHeight, 0), Quaternion.identity, transform);
            isSkiesShown = true;
        }
        leftWall.MoveWall(distance);
        rightWall.MoveWall(distance);
        Sky.transform.Translate(new Vector3(0, distance, 0));
        Floor.transform.Translate(new Vector3(0, distance, 0));
        _totalMovedDistance+=distance;
        return true;
    }

    public Vector3 GetConnectToWallPosition(Vector3 jointPosition)
    {
        var result = leftWall.GetConnectToWallPosition(jointPosition);
        if (result is null)
            result = rightWall.GetConnectToWallPosition(jointPosition);
        return result.Value;
    }

    public void AddOnDeadCallback(System.Action callback)
    {
        Floor.GetComponent<DeathPlate>().OnDead += callback;
    }

    public void Restart()
    {
        _spawner.DisableObstcales();
        GameObject[] Fathers = GameObject.FindGameObjectsWithTag("GrandDed");
        GameObject DedFather = Fathers[0];
        Object.Destroy(DedFather);
        GameObject newDed = Instantiate(DedPrefab, new Vector3(2.38f, 0.88f, 0), Quaternion.identity);
        newDed.gameObject.tag = "GrandDed";
        GetComponent<DragRigidbodyBetter>().ReInitDeda(newDed);
        menu.RestartReinitScript();
        Floor.transform.Translate( new Vector3(0, floorPosZ, 0) - Floor.transform.position);
        Sky.transform.Translate(new Vector3(0, skyPosZ, 0) - Sky.transform.position);
    }

    private void SpawnDed()
    {
        Ded.transform.position = new Vector3(Ded.transform.position.x, Sky.transform.position.y + 1.5f, Ded.transform.position.z);
        Ded.SetActive(true);
    }

    public void AddMenu(Menu menu)
    {
        Sky.GetComponent<DeathPlate>().AddMenu(menu);
        Floor.GetComponent<DeathPlate>().AddMenu(menu);
    }
}
