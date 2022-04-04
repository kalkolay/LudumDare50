using UnityEngine;
using UnityEngine.U2D;

public class BackgroundScript : MonoBehaviour
{
    [SerializeField]
    private GameObject ShapeOne;
    [SerializeField]
    private GameObject ShapeTwo;
    [SerializeField]
    private Camera MainCamera;

    private float _lastUpdateY;
    private float _segmentHeight;
    private GameObject _nextPieceToMove;

    void Start()
    {
        var spline = ShapeOne.GetComponent<SpriteShapeController>().spline;
        _segmentHeight = Mathf.Abs(spline.GetPosition(0).y - spline.GetPosition(1).y);
        _lastUpdateY =  MainCamera.transform.position.y;
        _nextPieceToMove = ShapeOne;
    }

    public void Move(float distance)
    {
        if (_lastUpdateY + _segmentHeight < MainCamera.transform.position.y)
        {
            _lastUpdateY = MainCamera.transform.position.y;
            _nextPieceToMove.transform.Translate(0, 2 * _segmentHeight, 0);
            _nextPieceToMove = _nextPieceToMove == ShapeOne ? ShapeTwo : ShapeOne;
        }
    }
}
