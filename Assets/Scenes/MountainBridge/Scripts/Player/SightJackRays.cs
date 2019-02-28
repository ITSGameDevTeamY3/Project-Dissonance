using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEditor;
using Camera = UnityEngine.Camera;
using Material = UnityEngine.Material;

[RequireComponent(typeof(SightJackController))]
public class SightJackRays : MonoBehaviour {

    public GameObject MainCamera;
    public GameObject Canvas;

    private GameObject _noise;
    private GameObject _targetCamera;
    private GameObject _enemy;

    private int _previousEnemy;

    public float Range = 5f;
    public float HorizontalAngle = 15f;
    public float VerticalAngle = 45f;
    public float Height = 0.29f;

    public bool IsDebug = true;

    private Vector3 _height;

    private Vector3 _centerRayDirection;
    private Vector3 _verticalRayDirection;
    private Vector3 _leftRayDirection;
    private Vector3 _rightRayDirection;

    private Ray _centerRay;
    private Ray _leftRay;
    private Ray _rightRay;

    private void Start ()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        Canvas = GameObject.FindWithTag("Canvas");

        if (MainCamera == null || Canvas == null)
        {
            Debug.LogError("SIGHTJACK ERROR: Could not find MainCamera or Canvas.");
            GetComponentInParent<SightJackController>().Deactivate();
        }

        _height = new Vector3(0, Height, 0);

        _centerRay = new Ray(transform.position + _height, transform.forward);
        _leftRay = new Ray(transform.position + _height, transform.forward);
        _rightRay = new Ray(transform.position + _height, transform.forward);

        // Get white noise from canvas
        _noise = Canvas.transform.Find("Noise").gameObject;
        _noise.SetActive(true);
    }

    private void Update ()
	{
		HandleInputRotation();
	    HandleCollision();
	}

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, Range);
    }

    private void OnDestroy()
    {
        _noise.SetActive(false);
        MainCamera.SetActive(true);
    }

    private void HandleInputRotation()
    {
        // Get input
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");

        // Get player's local direction
        var forward = transform.forward;
        var right = transform.right;
        var up = transform.up;

        forward.y = 0;
        right.y = 0;
        up.y = 0;
        forward.Normalize();
        right.Normalize();
        up.Normalize();

        // Calculate input direction
        _centerRayDirection = right * horizontalAxis + forward * verticalAxis;
        _leftRayDirection = Quaternion.AngleAxis(-HorizontalAngle, Vector3.up) * _centerRayDirection;
        _rightRayDirection = Quaternion.AngleAxis(HorizontalAngle, Vector3.up) * _centerRayDirection;

        // Increase range
        _centerRayDirection.x *= Range;
        _centerRayDirection.z *= Range;

        _leftRayDirection.x *= Range;
        _leftRayDirection.z *= Range;

        _rightRayDirection.x *= Range;
        _rightRayDirection.z *= Range;

        _centerRay.direction = _centerRayDirection;
        _leftRay.direction = _leftRayDirection;
        _rightRay.direction = _rightRayDirection;

        DrawDebug();
    }

    private void DrawDebug()
    {
        if (!IsDebug) return;
        Debug.DrawRay(transform.position + _height, _centerRayDirection, Color.blue);
        Debug.DrawRay(transform.position + _height, _leftRayDirection, Color.cyan);
        Debug.DrawRay(transform.position + _height, _rightRayDirection, Color.cyan);
    }

    private void HandleCollision()
    {
        RaycastHit hit;

        if ((Physics.Raycast(_leftRay, out hit) || Physics.Raycast(_rightRay, out hit)) && hit.collider.CompareTag("Enemy"))
        {
            if (hit.collider.gameObject.GetInstanceID() != _previousEnemy)
            {
                _enemy = hit.collider.gameObject;
                _previousEnemy = _enemy.GetInstanceID();

                _targetCamera = _enemy.transform.Find("Camera").gameObject;
                
                ToggleCamera(_targetCamera);
            }
        }
    }

    private void ToggleCamera(GameObject camera)
    {
        if (camera != null && camera.activeInHierarchy)
        {
            camera.SetActive(false);
            _noise.SetActive(true);
        }
        else if (!camera.activeInHierarchy && camera == null)
        {
            Debug.LogError("SIGHTJACK ERROR: " + camera.gameObject.name + " not found.");
            GetComponentInParent<SightJackController>().Deactivate();
        }
        else
        {
            camera.SetActive(true);
            _noise.SetActive(false);
        }
    }
}
