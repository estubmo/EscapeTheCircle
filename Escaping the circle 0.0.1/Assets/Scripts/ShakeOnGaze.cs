using UnityEngine;

/// <summary>
/// Script for a game object that grows dynamically when it senses the user's gaze.
/// </summary>
[RequireComponent(typeof(GazeAwareComponent))]
public class ShakeOnGaze : MonoBehaviour
{
	private static readonly Vector3 NormalScale = new Vector3(0.3f, 0.3f, 0.3f);
	private static readonly Vector3 LargeScale = new Vector3(0.6f, 0.6f, 0f);
	private Vector3 origPos;

	private float _scaleFactor = 0;
	private float timeToGazeBeforeShake;
	private float timer;
	private bool transformDirectionToggle;

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
			timeToGazeBeforeShake += Time.deltaTime;

			if (timeToGazeBeforeShake > 1.0f) {
				timer += Time.deltaTime;
				if (timer >= 0.04f) {
					if (!transformDirectionToggle) {
						transform.position = new Vector3 (transform.position.x + 0.01f, transform.position.y + 0.01f, transform.position.z + 0.01f);
						transformDirectionToggle = true;
					} else {
						transform.position = new Vector3 (transform.position.x - 0.01f, transform.position.y - 0.01f, transform.position.z - 0.01f);
						transformDirectionToggle = false;
					}
					timer = 0.0f;
				}
			}
		}
		else
		{
			//_scaleFactor = Mathf.Clamp01(_scaleFactor - speed * Time.deltaTime);
			timeToGazeBeforeShake = 0.0f;
		}
		//transform.localScale = Vector3.Slerp(NormalScale, LargeScale, _scaleFactor);
	}
}
