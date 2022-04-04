using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Text ValueDisplay;
    
    private string _id;
    private Slider _slider;

    void Awake()
    {
        _id = name;
        _slider = GetComponentInChildren<Slider>();
        float defaultValue = default;
        switch (_id)
        {
            case "slideDownSpeed":
                defaultValue = GameState.instance.GetSettings().slideDownSpeed;
                break;
            case "obstaclesFallSpeed":
                defaultValue = GameState.instance.GetSettings().obstaclesFallSpeed;
                break;
            case "testScrollSpeed":
                defaultValue = GameState.instance.GetSettings().testScrollSpeed;
                break;
            case "isObstaclesEnabled":
                defaultValue = GameState.instance.GetSettings().isObstaclesEnabled ? 1f : 0f;
                break;
            case "obstaclesInitSpawnDelay":
                defaultValue = GameState.instance.GetSettings().obstaclesInitSpawnDelay;
                break;
            case "obstaclesDelayModifier":
                defaultValue = GameState.instance.GetSettings().obstaclesDelayModifier;
                break;
            case "obstaclesSpawnAmount":
                defaultValue = GameState.instance.GetSettings().obstaclesSpawnAmount;
                break;
            case "obstaclesWidth":
                defaultValue = GameState.instance.GetSettings().obstaclesWidth;
                break;
            case "obstaclesHeight":
                defaultValue = GameState.instance.GetSettings().obstaclesHeight;
                break;
            case "wallsCloseDownAmount":
                defaultValue = GameState.instance.GetSettings().wallsCloseDownAmount;
                break;
            case "triggerWidth":
                defaultValue = GameState.instance.GetSettings().triggerWidth;
                break;
            case "smallObstacleWeight":
                defaultValue = GameState.instance.GetSettings().smallObstacleWeight;
                break;
            case "mediumObstacleWeight":
                defaultValue = GameState.instance.GetSettings().mediumObstacleWeight;
                break;
            case "largeObstacleWeight":
                defaultValue = GameState.instance.GetSettings().largeObstacleWeight;
                break;
            case "obstacleMaxSpeed":
                defaultValue = GameState.instance.GetSettings().obstacleMaxSpeed;
                break;
        }
        _slider.value = defaultValue;
        ValueDisplay.text = defaultValue.ToString();
    }

    public void OnValueShange(float newValue)
    {
        switch (_id)
        {
            case "slideDownSpeed":
                GameState.instance.GetSettings().slideDownSpeed = _slider.value;
                break;
            case "obstaclesFallSpeed":
                GameState.instance.GetSettings().obstaclesFallSpeed = _slider.value;
                break;
            case "testScrollSpeed":
                GameState.instance.GetSettings().testScrollSpeed = _slider.value;
                break;
            case "isObstaclesEnabled":
                GameState.instance.GetSettings().isObstaclesEnabled = _slider.value == 1;
                break;
            case "obstaclesInitSpawnDelay":
                GameState.instance.GetSettings().obstaclesInitSpawnDelay = _slider.value;
                break;
            case "obstaclesDelayModifier":
                GameState.instance.GetSettings().obstaclesDelayModifier = _slider.value;
                break;
            case "obstaclesSpawnAmount":
                GameState.instance.GetSettings().obstaclesSpawnAmount = _slider.value;
                break;
            case "obstaclesWidth":
                GameState.instance.GetSettings().obstaclesWidth = _slider.value;
                break;
            case "obstaclesHeight":
                GameState.instance.GetSettings().obstaclesHeight = _slider.value;
                break;
            case "wallsCloseDownAmount":
                GameState.instance.GetSettings().wallsCloseDownAmount = _slider.value;
                break;
            case "triggerWidth":
                GameState.instance.GetSettings().triggerWidth = _slider.value;
                break;
            case "smallObstacleWeight":
                GameState.instance.GetSettings().smallObstacleWeight = _slider.value;
                break;
            case "mediumObstacleWeight":
                 GameState.instance.GetSettings().mediumObstacleWeight = _slider.value;
                break;
            case "largeObstacleWeight":
                GameState.instance.GetSettings().largeObstacleWeight = _slider.value;
                break;
            case "obstacleMaxSpeed":
                GameState.instance.GetSettings().obstacleMaxSpeed = _slider.value;
                break;
        }
        ValueDisplay.text = _slider.value.ToString();
    }
}
