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
    [SerializeField]
    private GameObject Sky;
    [SerializeField]
    private GameObject Floor;
    [SerializeField]
    private GameObject Ded;

    [System.NonSerialized]
    public static GameState instance = null;
    [System.NonSerialized]
    public int currentHeight = 1;

    private ObstacleSpawnerScript _spawner;
    private float _totalMovedDistance = 0;

    public GameObject DedPrefab;
    public Menu menu;

    public int Score;

    void Awake()
    {
        instance = this;
        var spawnerGO = Instantiate(spawner, new Vector3(0, 0, 1), Quaternion.identity);
        _spawner = spawnerGO.GetComponent<ObstacleSpawnerScript>();
        Sky.GetComponent<DeathPlate>().OnDead += SpawnDed;
        Sky.SetActive(false);
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
        StartCoroutine(WaitForCameraShiftToDniwe());
    }

    private IEnumerator WaitForCameraShiftToDniwe()
    {
        while (Camera.main.transform.position.y > 0.001f)
        {
            yield return new WaitForEndOfFrame();
        }
        GameObject[] Fathers = GameObject.FindGameObjectsWithTag("GrandDed");
        GameObject DedFather = Fathers[0];
        Object.Destroy(DedFather);
        GameObject newDed = Instantiate(DedPrefab, new Vector3(2.38f, 0.88f, 0), Quaternion.identity);
        newDed.gameObject.tag = "GrandDed";
        GetComponent<DragRigidbodyBetter>().ReInitDeda(newDed);
        menu.RestartReinitScript();
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
