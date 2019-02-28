using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;
using Camera = UnityEngine.Camera;
using GameObject = UnityEngine.GameObject;
using Material = UnityEngine.Material;

[RequireComponent(typeof(SightJackController))]
public class SightJackView : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Canvas;

    private GameObject _centerView;
    private GameObject _noise;
    private GameObject _targetCamera;

    private Stack<GameObject> _enemies;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    public Mesh View;

    public Material CenterMaterial;

    public bool IsDebug = false;
    public bool Scale = false;

    private void Start()
    {
        _enemies = new Stack<GameObject>();

        MainCamera = GameObject.FindWithTag("MainCamera");
        Canvas = GameObject.FindWithTag("Canvas");

        if (MainCamera == null || Canvas == null)
        {
            Debug.LogError("SIGHT-JACK ERROR: Could not find MainCamera or Canvas.");
            GetComponentInParent<SightJackController>().Deactivate();
        }
        
        CreateView(GetComponent<Camera>());

        // Get white noise from canvas
        _noise = Canvas.transform.Find("Noise").gameObject;
        _noise.SetActive(true);
    }

    private void Update()
    {
        HandleInputRotation();

        if (IsDebug)
            _meshRenderer.enabled = true;
        else
            _meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _enemies.Push(other.gameObject);
            _targetCamera = other.transform.Find("Camera").gameObject;

            ToggleNoise();
            ToggleCamera(MainCamera);
            ToggleCamera(_targetCamera);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _enemies.Pop();
            ToggleCamera(_targetCamera);
            ToggleCamera(MainCamera);
            ToggleNoise();
        }
    }

    private void OnDestroy()
    {
        MainCamera.SetActive(true);
        _noise.SetActive(false);

        Destroy(_centerView);
        Destroy(_meshRenderer);
        Destroy(_meshFilter);
        Destroy(_meshCollider);

        _meshRenderer.enabled = false;

        transform.localScale = new Vector3(1, 1, 1);
    }

    private void CreateView(Camera camera)
    {
        if (camera != null)
        {
            var controller = GetComponent<SightJackController>();

            IsDebug = controller.IsDebug;
            Scale = controller.Scale;

            camera.fieldOfView = controller.FieldOfView;
            camera.farClipPlane = controller.Range;

            View = camera.GenerateFrustumMesh();
            
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();            
            _meshRenderer.receiveShadows = false;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.material = controller.Frustum;
            CenterMaterial = controller.Center;

            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = View;

            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = View;
            _meshCollider.convex = true;
            _meshCollider.isTrigger = true;

            transform.localScale = new Vector3(1, controller.Height, 1);

            CreateViewCenter();
        }
        else
        {
            Debug.LogError("SIGHT-JACK ERROR: Camera frustum not found.");
        }
    }

    private void CreateViewCenter()
    {
        _centerView = new GameObject("Center");
        _centerView.transform.rotation = transform.rotation;
        _centerView.transform.position = transform.position;
        _centerView.transform.parent = transform;
        _centerView.AddComponent<SightJackCenter>();
    }

    private void HandleInputRotation()
    {
        // Get input
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");

        // Calculate direction
        transform.localEulerAngles = new Vector3(0, Mathf.Atan2(horizontalAxis, verticalAxis) * 180 / Mathf.PI, 0);

        if (Scale)
            transform.localScale = new Vector3(Mathf.Abs(horizontalAxis + verticalAxis) * 1, 1, Mathf.Abs(verticalAxis + horizontalAxis) * 1);
    }    

    private void ToggleCamera(GameObject camera)
    {
        if (camera != null && camera.activeInHierarchy)
        {
            camera.GetComponent<AudioListener>().enabled = false;
            camera.SetActive(false);
        }
        else if (!camera.activeInHierarchy && camera == null)
        {
            Debug.LogError("SIGHTJACK ERROR: " + camera.gameObject.name + " not found.");
            GetComponentInParent<SightJackController>().Deactivate();
        }
        else
        {            
            camera.SetActive(true);
            camera.GetComponent<AudioListener>().enabled = true;
        }
    }

    private void ToggleNoise()
    {
        _noise.SetActive(!_noise.activeInHierarchy);
    }
}
