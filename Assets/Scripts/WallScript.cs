using UnityEngine;
using UnityEngine.U2D;

public class WallScript : MonoBehaviour
{
    [SerializeField]
    private bool IsLeft;
    [SerializeField]
    private SpriteShapeController[] Walls;
    [SerializeField]
    private SpriteShapeController[] Triggers;

    [System.NonSerialized]
    public float HighestCorner;

    private int[] _positionsToUpdate;
    private int[] _opposePositionsToUpdate;
    private int _deltaModifier;
    private float _delta;
    private float _lastUpdateY;
    private float _segmentHeight;
    private int _nextUpdatedWallIndex;

    void Start()
    {
        _positionsToUpdate = IsLeft ? new int[] { 2, 3 } : new int[] { 1, 0 };
        _opposePositionsToUpdate = IsLeft ? new int[] { 1, 0 } : new int[] { 2, 3 };
        _segmentHeight = Mathf.Abs(Walls[0].spline.GetPosition(_positionsToUpdate[0]).y - Walls[0].spline.GetPosition(_positionsToUpdate[1]).y);
        _deltaModifier = IsLeft ? 1 : -1;
        _delta = _deltaModifier * GameState.instance.GetSettings().wallsCloseDownAmount;
        UpdatePositions(Walls[0].spline, new int[] { _positionsToUpdate[0] });
        _nextUpdatedWallIndex = -1;
        UpdateCorner(Walls[0]);
        for (int i = 0; i < Walls.Length; i++)
            UpdateTrigger(i);
    }

    void Update()
    {
        //MoveWall(GameState.instance.GetSettings().testScrollSpeed);
    }

    public void MoveWall(float distance)
    {
        transform.Translate(new Vector3(0, -distance, 0));
        if (_lastUpdateY - _segmentHeight - 0.1f * _segmentHeight > transform.position.y)
            MoveWall();
    }

    private void UpdatePositions(Spline spline, int[] positionsToUpdate)
    {
        _lastUpdateY = transform.position.y + 0.1f * _segmentHeight;
        foreach (var position in positionsToUpdate)
        {
            var pointAtPosition = spline.GetPosition(position);
            spline.SetPosition(position, new Vector3(pointAtPosition.x + _delta, pointAtPosition.y, pointAtPosition.z));
        }
    }

    private void MoveWall()
    {
        if (_nextUpdatedWallIndex < 0)
        {
            _nextUpdatedWallIndex = Walls.Length - 1;
            GameState.instance.SpeedUp();
        }
        var wallToUpdate = Walls[_nextUpdatedWallIndex];
        wallToUpdate.transform.Translate(0, Walls.Length * _segmentHeight, 0);
        Triggers[_nextUpdatedWallIndex].transform.Translate(0, Walls.Length * _segmentHeight, 0);
        UpdatePositions(wallToUpdate.spline, _positionsToUpdate);
        UpdateCorner(wallToUpdate);
        UpdateTrigger(_nextUpdatedWallIndex);
        _nextUpdatedWallIndex--;
        if (IsLeft)
            GameState.instance.currentHeight++;
    }

    private void UpdateCorner(SpriteShapeController wall)
    {
        HighestCorner = wall.spline.GetPosition(_positionsToUpdate[0]).x + wall.transform.localPosition.x + transform.position.x;
    }

    private void UpdateTrigger(int index)
    {
        var wall = Walls[index];
        var trigger = Triggers[index];
        for (int positionIndex = 0; positionIndex < _positionsToUpdate.Length; positionIndex++)
        {
            var positionToClayTo = wall.spline.GetPosition(_positionsToUpdate[positionIndex]);
            trigger.spline.SetPosition(_positionsToUpdate[positionIndex], new Vector3(positionToClayTo.x + _deltaModifier * GameState.instance.GetSettings().triggerWidth, positionToClayTo.y, positionToClayTo.z));
            trigger.spline.SetPosition(_opposePositionsToUpdate[positionIndex], positionToClayTo);
        }
    }
}
