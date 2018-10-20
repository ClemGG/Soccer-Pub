using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class MultipleTargetCam : MonoBehaviour {

	[HideInInspector] public List<Transform> targets;

    [Space(10)]
    [Header("Camera Settings :")]
    [Space(10)]

    [SerializeField] private Vector3 offset;
	[SerializeField] private float smoothTime = .5f;

    [Space(10)]

    [Range(1, 10)] [SerializeField] private float dampingX = 1f;
    [Range(1, 10)] [SerializeField] private float dampingY = 1f;
    [Range(1, 10)] [SerializeField] private float dampingZ = 1f;

    [Space(10)]

    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
	[SerializeField] private float zoomLimiter; // Permet de diviser la distance maximale entre deux joueurs par la taille du terrain

    private Vector3 velocity;
    private Camera cam;

    [Space(10)]
    [Header("Camera Gizmos :")]
    [Space(10)]

    [SerializeField] private bool showGizmos;
    private Vector3 newCamPosGizmo;


    private void Start()
    {
        cam = GetComponent<Camera>();
    }








    void LateUpdate(){
	
		if (targets.Count == 0) {
			return;
		}


		MoveCamera ();
		ZoomOnTargets ();


	}







	void MoveCamera()
	{
        Vector3 centerPoint = GetCenterPoint ();
        Vector3 centerPointXY = new Vector3(centerPoint.x / dampingX, centerPoint.y / dampingY, centerPoint.z / dampingZ);
        Vector3 newPosition = centerPointXY + offset;

        newCamPosGizmo = newPosition;

		transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
	}







	void ZoomOnTargets()
	{
        float coef = GetGreatestDistance() / zoomLimiter;

        float zoomCoef = Mathf.Clamp01(coef * zoomSpeed * 100f * Time.deltaTime);
        //Debug.Log(zoomCoef);

        float newZoom = Mathf.Lerp(maxZoom, minZoom, coef);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, zoomCoef);
        
	}






	float GetGreatestDistance()
	{
		var bounds = new Bounds (targets [0].position, Vector3.zero);
		for (int i = 0; i < targets.Count; i++) 
		{
			bounds.Encapsulate (targets [i].position);
		}
		return bounds.size.x;
	}





	Vector3 GetCenterPoint()
	{
		if (targets.Count == 1) {
			return targets [0].position;
		}

		var bounds = new Bounds (targets [0].position, Vector3.zero);

		for (int i = 0; i < targets.Count; i++) 
		{
			bounds.Encapsulate (targets [i].position);
		}
		return bounds.center;

	}


    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(newCamPosGizmo, .3f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(offset, .3f);
        }
    }
}
