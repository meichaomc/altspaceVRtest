using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
	// This is a sensitivity parameter that should adjust how sensitive the mouse control is.
	public float Sensitivity;

	// This is a scale factor that determines how much to scale down the cursor based on its collision distance.
	public float DistanceScaleFactor;
	
	// This is the layer mask to use when performing the ray cast for the objects.
	// The furniture in the room is in layer 8, everything else is not.
	private const int ColliderMask = (1 << 8);

	// This is the Cursor game object. Your job is to update its transform on each frame.
	private GameObject Cursor;

	// This is the Cursor mesh. (The sphere.)
	private MeshRenderer CursorMeshRenderer;

	// This is the scale to set the cursor to if no ray hit is found.
	private Vector3 DefaultCursorScale = new Vector3(10.0f, 10.0f, 10.0f);

	// Maximum distance to ray cast.
	private const float MaxDistance = 100.0f;

	// Sphere radius to project cursor onto if no raycast hit.
	private const float SphereRadius = 10.0f;//Chao on 11.12.2015: I changed it from 1000 to 10, 1000 seems too far aray

	// Chao on 11.12.2015: These are used to handle rotation on sphere
	float rotationX = 0f;
	float rotationY = 0f;

	// Chao on 11.12.2015: Max and min to clamp the cursor movement
	float minClamp = -50f;
	float maxClamp = 50f;

	//force on click
	public float force;
	
    void Awake() {
		Cursor = transform.Find("Cursor").gameObject;
		CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
        CursorMeshRenderer.renderer.material.color = new Color(0.0f, 0.8f, 1.0f);
    }	

	void Update()
	{
		// TODO: Handle mouse movement to update cursor position.

		rotationX += Input.GetAxis("Mouse X") * Sensitivity;
		rotationY += Input.GetAxis("Mouse Y") * Sensitivity;		
		rotationY = Mathf.Clamp(rotationY, minClamp, maxClamp);	
		rotationX = Mathf.Clamp(rotationX, minClamp, maxClamp);	
		Quaternion rotation = Quaternion.Euler(-rotationY, rotationX, 0);
		Vector3 position = rotation * new Vector3(0f, 0f, SphereRadius) + Camera.main.transform.position;
		Vector3 newScale = DefaultCursorScale;
				

		// TODO: Perform ray cast to find object cursor is pointing at.
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.transform.position, -Camera.main.transform.position + Cursor.transform.position, out hit) && hit.collider.gameObject.layer == 8) {
			newScale = DefaultCursorScale * (hit.distance * DistanceScaleFactor + 1.0f) / 2.0f;
			if (Input.GetMouseButtonDown(0)){
				hit.collider.rigidbody.AddForceAtPosition(Camera.main.transform.forward * force, hit.point);
				audio.Play();
			}
			
		} else {
			newScale = DefaultCursorScale;
		}
		// TODO: Update cursor transform.
		//Cursor.transform.rotation = rotation;
		Cursor.transform.position = position;
		Cursor.transform.localScale = newScale;

		var cursorHit = new RaycastHit();/* Your cursor hit code should set this properly. */;
		cursorHit = hit;
		// Update highlighted object based upon the raycast.
		if (cursorHit.collider != null)
		{
			Selectable.CurrentSelection = cursorHit.collider.gameObject;
		}
		else
		{
			Selectable.CurrentSelection = null;
		}
	}

}
