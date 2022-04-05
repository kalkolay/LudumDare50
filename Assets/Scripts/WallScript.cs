using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class WallScript : MonoBehaviour
{
    [SerializeField]
    private bool IsLeft;
    [SerializeField]
    private SpriteShapeController[] Walls;
    [SerializeField]
    private SpriteShapeController[] Colliders;

    [System.NonSerialized]
    public Vector2 HighestCorner;

    private int[] _positionsToUpdate;
    private int _deltaModifier;
    private float _delta;
    private float _lastUpdateY;
    private float _segmentHeight;
    private int _nextUpdatedWallIndex;

    void Start()
    {
        _positionsToUpdate = IsLeft ? new int[] { 2, 3 } : new int[] { 1, 0 };
        _segmentHeight = Mathf.Abs(Walls[0].spline.GetPosition(_positionsToUpdate[0]).y - Walls[0].spline.GetPosition(_positionsToUpdate[1]).y);
        _deltaModifier = IsLeft ? 1 : -1;
        _delta = _deltaModifier * GameState.instance.GetSettings().wallsCloseDownAmount;
        UpdatePositions(Walls[0].spline, new int[] { _positionsToUpdate[0] }, Walls[0].gameObject);
        UpdatePositions(Colliders[0].spline, new int[] { _positionsToUpdate[0] }, Colliders[0].gameObject);
        _nextUpdatedWallIndex = -1;
        UpdateCorner(Walls[0]);
    }

    public void MoveWall(float distance)
    {
        if (_lastUpdateY + _segmentHeight + 0.1f * _segmentHeight < Camera.main.transform.position.y)
            MoveWall();
    }

    private void UpdatePositions(Spline spline, int[] positionsToUpdate, GameObject gameObject)
    {
        _lastUpdateY = Camera.main.transform.position.y + 0.1f * _segmentHeight;
        foreach (var position in positionsToUpdate)
        {
            var pointAtPosition = spline.GetPosition(position);
            spline.SetPosition(position, new Vector3(pointAtPosition.x + _delta, pointAtPosition.y, pointAtPosition.z));
        }
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    private void MoveWall()
    {
        if (_nextUpdatedWallIndex < 0)
        {
            _nextUpdatedWallIndex = Walls.Length - 1;
            GameState.instance.SpeedUp();
        }
        var wallToUpdate = Walls[_nextUpdatedWallIndex];
        var colliderToUpdate = Colliders[_nextUpdatedWallIndex];
        wallToUpdate.transform.Translate(0, Walls.Length * _segmentHeight, 0);
        colliderToUpdate.transform.Translate(0, Walls.Length * _segmentHeight, 0);
        UpdatePositions(wallToUpdate.spline, _positionsToUpdate, wallToUpdate.gameObject);
        UpdatePositions(colliderToUpdate.spline, _positionsToUpdate, colliderToUpdate.gameObject);
        UpdateCorner(wallToUpdate);
        _nextUpdatedWallIndex--;
        if (IsLeft)
            GameState.instance.currentHeight++;
    }

    private void UpdateCorner(SpriteShapeController wall)
    {
        HighestCorner = new Vector2(PointXToGlobalX(wall.spline.GetPosition(_positionsToUpdate[0]).x, wall), PointYToGlobalY(wall.spline.GetPosition(_positionsToUpdate[0]).y, wall));
    }

    private float PointXToGlobalX(float x, SpriteShapeController wall)
    {
        return x + wall.transform.localPosition.x + transform.position.x;
    }

    private float PointYToGlobalY(float y, SpriteShapeController wall)
    {
        return y + wall.transform.position.y;
    }

    public Vector3? GetConnectToWallPosition(Vector3 jointPosition)
    {
        if (IsLeft && jointPosition.x > 0)
            return null;
        var wall = Walls.FirstOrDefault(x =>
        {
            var firstPosition = PointYToGlobalY(x.spline.GetPosition(_positionsToUpdate[0]).y, x);
            var secondPosition = PointYToGlobalY(x.spline.GetPosition(_positionsToUpdate[1]).y, x);
            var checkPosition = jointPosition.y;
            return firstPosition - checkPosition > Mathf.Epsilon && secondPosition - checkPosition < - Mathf.Epsilon;
        });
        var position1 = wall.spline.GetPosition(_positionsToUpdate[0]);
        var position2 = wall.spline.GetPosition(_positionsToUpdate[1]);
        var position1X = PointXToGlobalX(position1.x, wall);
        var position2X = PointXToGlobalX(position2.x, wall);
        var position1Y = PointYToGlobalY(position1.y, wall);
        var position2Y = PointYToGlobalY(position2.y, wall);
        var resultX = ((jointPosition.y - position1Y) * (position2X - position1X)) / (position2Y - position1Y) + position1X;
        return new Vector3(resultX, jointPosition.y, jointPosition.z);
    }
}
