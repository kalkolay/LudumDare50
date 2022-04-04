using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
public class SettingsSO : ScriptableObject
{
    public float slideDownSpeed = 0f;
    public float obstaclesFallSpeed = 0.001f;
    public float testScrollSpeed = 0.001f;
    public bool isObstaclesEnabled = true;
    public float obstaclesInitSpawnDelay = 1;
    public float obstaclesDelayModifier = 0.9f;
    public float obstaclesSpawnAmount = 1;
    public float obstaclesWidth = 2;
    public float obstaclesHeight = 2;
    public float wallsCloseDownAmount = 0.3f;
    public float triggerWidth = 0.1f;
    public float maxHeight = 50f;
    public float smallObstacleWeight = 10f;
    public float mediumObstacleWeight = 10f;
    public float largeObstacleWeight = 10f;
    public float obstacleMaxSpeed = 5f;
}