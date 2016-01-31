using UnityEngine;

/// <summary>
/// Script for a game object that grows dynamically when it senses the user's gaze.
/// </summary>
[RequireComponent(typeof(GazeAwareComponent))]
public class GrowOnGaze : MonoBehaviour
{
	private static readonly Vector3 NormalScale = new Vector3(0.3f, 0.3f, 0.3f);
	private static readonly Vector3 LargeScale = new Vector3(0.6f, 0.6f, 0f);
	private Vector3 origPos;

	private float _scaleFactor = 0;

	public float speed = 10.0f;
	private GazeAwareComponent _gazeAwareComponent;

	protected void Start()
	{
		_gazeAwareComponent = GetComponent<GazeAwareComponent>();
		origPos = transform.position;
	}

	/// <summary>
	/// Update interactor bounds and transform
	/// </summary>
	protected void Update()
	{
		// Update the scale factor depending on whether the eye-gaze is on the object or not.
		if (_gazeAwareComponent.HasGaze)
		{
			//transform.position = transform.position.x + 1;
			//transform.Rotate(Vector3.down);
			//_scaleFactor = Mathf.Clamp01(_scaleFactor + speed * Time.deltaTime);
		}
			else
		{
			//_scaleFactor = Mathf.Clamp01(_scaleFactor - speed * Time.deltaTime);
			transform.localPosition = origPos;
		}
		transform.localScale = Vector3.Slerp(NormalScale, LargeScale, _scaleFactor);
	}
}
