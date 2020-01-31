using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplayerZoomCamera : MonoBehaviour
{
	private enum Zoom { Out, In, None };

	/// <summary>The world coordinate of the bottom left point of the camera view.</summary>
	public Vector2 MinVisiblePos
	{
		get
		{
			Vector2 pos = _targetPosition;
			pos.x -= LevelHorizontalLength;
			pos.y -= LevelVerticalLength;
			return pos;
		}
	}

	/// <summary>The world coordinate of the top right point of the camera view.</summary>
	public Vector2 MaxVisiblePos
	{
		get
		{
			Vector2 pos = _targetPosition;
			pos.x += LevelHorizontalLength;
			pos.y += LevelVerticalLength;
			return pos;
		}
	}

	/// <summary>The distance between the top most and bottom most points visible to the camera.</summary>
	public float LevelVerticalLength => _currentZoom;

	/// <summary>The distance between the left most and right most points visible to the camera.</summary>
	public float LevelHorizontalLength => _currentZoom * Screen.width / Screen.height;

	[SerializeField]
	private Vector3 _cameraOffset = new Vector3(0, 0, -10f);

	[SerializeField]
	private Transform _bottomLeftIndicator;

	[SerializeField]
	private Transform _topRightIndicator;

	[SerializeField, Tooltip("Approximately the time it will take the camera to smoothly reach the target. A smaller value will reach the target faster.")]
	private float _smoothTime = 0.3f;

	private Vector3 _targetPosition = Vector3.zero;
	private Vector3 _velocity = Vector3.zero;

	private Vector2 _levelMinimumPoint = Vector2.zero, _levelMaximumPoint = Vector2.zero;
	private Vector2 _cameraMinimumPoint => _levelMinimumPoint + new Vector2(LevelHorizontalLength, LevelVerticalLength);
	private Vector2 _cameraMaximumPoint => _levelMaximumPoint - new Vector2(LevelHorizontalLength, LevelVerticalLength);

	private Player[] _players;

	private const float MAX_ZOOM_LEVEL = 14f;
	private const float MIN_ZOOM_LEVEL = 8f;
	private const float DELTA_ZOOM = 0.05f;

	private Camera _cam;
	private float _currentZoom;
	private List<Zoom> _zoomStates = new List<Zoom>();

	private void Awake()
	{
		_cam = GetComponent<Camera>();
		_currentZoom = _cam.orthographicSize;

		AcquirePlayerRefs();

		_levelMinimumPoint = _bottomLeftIndicator.position;
		_levelMaximumPoint = _topRightIndicator.position;

		//PositionCameraAtSpawn();

		_bottomLeftIndicator.gameObject.SetActive(false);
		_topRightIndicator.gameObject.SetActive(false);
	}

	private void AcquirePlayerRefs()
	{
		_players = FindObjectsOfType<Player>();
	}

	private void LateUpdate()
	{
		ZoomView();
		PositionCamera();
	}

	//private void PositionCameraAtSpawn()
	//{
	//	SpawnPoint[] spawns = FindObjectsOfType<SpawnPoint>();

	//	if (spawns == null || spawns.Length == 0)
	//		return;

	//	float maxY = spawns.Max(p => p.transform.position.y);
	//	Vector3 avgPos = spawns.Select(p => p.transform.position).Aggregate((total, next) => total += next) / spawns.Length;

	//	avgPos.y = Mathf.Max(avgPos.y, maxY - LevelVerticalLength / 2);

	//	Vector3 clamped = avgPos + _cameraOffset;
	//	clamped.x = Mathf.Clamp(clamped.x, _cameraMinimumPoint.x, _cameraMaximumPoint.x);
	//	clamped.y = Mathf.Clamp(clamped.y, _cameraMinimumPoint.y, _cameraMaximumPoint.y);
	//	transform.position = clamped;
	//}

	private void ZoomView()
	{
		if (_zoomStates.Count == 0) return;

		if (_zoomStates.Any(z => z == Zoom.Out) && _currentZoom < MAX_ZOOM_LEVEL)
		{
			float newZoom = _currentZoom + DELTA_ZOOM;
			_currentZoom = newZoom > MAX_ZOOM_LEVEL ? MAX_ZOOM_LEVEL : newZoom;
		}
		else if (_zoomStates.All(z => z == Zoom.In) && _currentZoom > MIN_ZOOM_LEVEL)
		{
			float newZoom = _currentZoom - DELTA_ZOOM;
			_currentZoom = newZoom < MIN_ZOOM_LEVEL ? MIN_ZOOM_LEVEL : newZoom;
		}

		_cam.orthographicSize = _currentZoom;

		_zoomStates = new List<Zoom>();
	}

	private void PositionCamera()
	{
		if (_players == null || _players.Length == 0) return;

		Vector3 avgPos;
		if (_players.Length == 1)
		{
			avgPos = _players[0].transform.position;
		}
		else
		{
			float yMax = _players.Max(p => p.transform.position.y) - (LevelVerticalLength / 2);
			float yMin = _players.Min(p => p.transform.position.y) + (LevelVerticalLength / 2);
			float twoThirdsCamHeight = LevelVerticalLength / 3 * 2;
			float halfCamHeight = LevelVerticalLength / 2;

			avgPos = _players.Select(p => p.transform.position).Aggregate((total, next) => total += next) / _players.Length;

			if (yMax - yMin > twoThirdsCamHeight)
			{
				_zoomStates.Add(Zoom.Out);
			}
			else if (yMax - yMin > halfCamHeight)
			{
				_zoomStates.Add(Zoom.None);
			}
			else if (_currentZoom >= MAX_ZOOM_LEVEL)
			{
				avgPos.y = Mathf.Max(avgPos.y, yMax);
			}
		}

		Vector3 clamped = avgPos + _cameraOffset;
		clamped.x = Mathf.Clamp(clamped.x, _cameraMinimumPoint.x, _cameraMaximumPoint.x);
		clamped.y = Mathf.Clamp(clamped.y, _cameraMinimumPoint.y, _cameraMaximumPoint.y);
		_targetPosition = clamped;

		transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _smoothTime);
	}

	/// <summary>Constrains an object to be fully within the view of the camera.</summary>
	/// <param name="pos">The world position of the object to be constrained.</param>
	/// <param name="objBounds">The visual bounds of the object to be constrained.</param>
	/// <param name="constrainAxisY">Should the object be constrained along the camera Y axis.</param>
	/// <returns>The constrained world position of the object.</returns>
	public Vector3 ConstrainToView(Vector3 pos, Vector3 objBounds, bool constrainAxisY = false)
	{
		Vector3 camPos = _targetPosition;

		float xMin = camPos.x - LevelHorizontalLength + objBounds.x;
		float xMax = camPos.x + LevelHorizontalLength - objBounds.x;

		SetZoomState(pos.x, xMin, xMax);

		pos.x = Mathf.Clamp(pos.x, xMin, xMax);

		float yMin = pos.y;
		float yMax = camPos.y + LevelVerticalLength - objBounds.y;

		if (constrainAxisY)
		{
			yMin = camPos.y - LevelVerticalLength + objBounds.y;
		}

		pos.y = Mathf.Clamp(yMin, pos.y, yMax);

		return pos;
	}

	private void SetZoomState(float constrainedObjX, float minVisibleX, float maxVisibleX)
	{
		float zoomInPadding = 2f;
		float zoomOutPadding = zoomInPadding / 2f;

		bool onLeftLevelEdge = transform.position.x - _cameraMinimumPoint.x < 1f && constrainedObjX <= transform.position.x;
		bool onRightLevelEdge = _cameraMaximumPoint.x - transform.position.x < 1f && constrainedObjX >= transform.position.x;

		bool onLeftScreenEdge = constrainedObjX <= minVisibleX + zoomOutPadding;
		bool onRightScreenEdge = constrainedObjX >= maxVisibleX - zoomOutPadding;

		bool inCenterScreen = constrainedObjX <= maxVisibleX - zoomInPadding && constrainedObjX >= minVisibleX + zoomInPadding;

		if (onLeftLevelEdge || onRightLevelEdge || inCenterScreen)
		{
			_zoomStates.Add(Zoom.In);
		}
		else if (onLeftScreenEdge || onRightScreenEdge)
		{
			_zoomStates.Add(Zoom.Out);
		}
		else
		{
			_zoomStates.Add(Zoom.None);
		}
	}

	/// <summary>Callback to draw gizmos that are pickable and always drawn.</summary>
	private void OnDrawGizmos()
	{
		if (_bottomLeftIndicator && _topRightIndicator)
		{
			Vector3 bottomLeft = _bottomLeftIndicator.transform.position;
			Vector3 topRight = _topRightIndicator.transform.position;
			Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y);
			Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(bottomLeft, topLeft + Vector3.up);
			Gizmos.DrawLine(bottomLeft, bottomRight + Vector3.right);
			Gizmos.DrawLine(topRight, topLeft - Vector3.right);
			Gizmos.DrawLine(topRight, bottomRight + Vector3.down);
		}
	}
}