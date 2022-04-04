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
    private GameObject DevMenu;
    [SerializeField]
    private BackgroundScript Background;
    [SerializeField]
    private GameObject Sky;
    [SerializeField]
    private GameObject Floor;
    [SerializeField]
    private GameObject Ded;
    [SerializeField]
    private UnityEngine.UI.Text CurrentHeightLabel;

    [System.NonSerialized]
    public static GameState instance = null;
    [System.NonSerialized]
    public int currentHeight = 1;

    private ObstacleSpawnerScript _spawner;
    private float _totalMovedDistance = 0;

    public GameObject DedPrefab;
    public Menu menu;

    void Awake()
    {
        instance = this;
        var spawnerGO = Instantiate(spawner, new Vector3(0, 0, 1), Quaternion.identity);
        _spawner = spawnerGO.GetComponent<ObstacleSpawnerScript>();
        Sky.GetComponent<DeathPlate>().OnDead += SpawnDed;
        Sky.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            Time.timeScale = DevMenu.activeSelf ? 1 : 0;
            DevMenu.SetActive(!DevMenu.activeSelf);
        }
        CurrentHeightLabel.text = currentHeight.ToString();
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

    public void OnCameraMove(float distance)
    {
        Background.Move(distance);
        if (_totalMovedDistance >= settingsSo.maxHeight)
        {
            Sky.SetActive(true);
            return;
        }
        leftWall.MoveWall(distance);
        rightWall.MoveWall(distance);
        Sky.transform.Translate(new Vector3(0, distance, 0));
        Floor.transform.Translate(new Vector3(0, distance, 0));
        _totalMovedDistance+=distance;
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
        Sky.GetComponent<DeathPlate>().OnDead += callback;
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
    }
}
