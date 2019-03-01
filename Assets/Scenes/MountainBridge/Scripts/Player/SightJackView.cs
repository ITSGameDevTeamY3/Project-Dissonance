using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Camera = UnityEngine.Camera;
using GameObject = UnityEngine.GameObject;
using Material = UnityEngine.Material;

[RequireComponent(typeof(SightJackController))]
public class SightJackView : MonoBehaviour
{
    private GameObject _mainCamera;
    private GameObject _canvas;

    private Camera _uiCamera;

    private GameObject _noise;
    private GameObject _centerView;

    private List<GameObject> _enemies;
    private GameObject _targetEnemy;
    private GameObject _currentEnemy;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    public Mesh View;

    public Material CenterMaterial;

    public bool IsDebug = false;
    public bool Scale = false;

    private EventInstance _noiseEvent;

    private void Start()
    {
        _enemies = new List<GameObject>();

        try
        {
            _mainCamera = GameObject.FindWithTag("MainCamera");
            _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            _canvas = GameObject.FindWithTag("Canvas");
        }
        catch (NullReferenceException exception)
        {
            Debug.LogError("SIGHT-JACK ERROR: Could not find required objects.");
            GetComponentInParent<SightJackController>().Deactivate();
        }
        
        CreateView(GetComponent<Camera>());

        // Get white noise from canvas
        _noise = _canvas.transform.Find("Noise").gameObject;
        _noise.SetActive(true);

        _noiseEvent = RuntimeManager.CreateInstance("event:/Master/SFX_Events/SightJacking/WhiteNoise");
        _noiseEvent.start();
    }

    private void Update()
    {
        HandleInputRotation();
        HandleMultipleEnemies();
        BalanceNoise();
        AutomateSounds();

        if (IsDebug)
            _meshRenderer.enabled = true;
        else
            _meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !_enemies.Contains(other.gameObject))
        {
            _currentEnemy = other.gameObject; // For noise shader
            _enemies.Add(other.gameObject);

            if (_enemies.Count <= 1)
            {
                ToggleCamera(_mainCamera);
                ToggleCamera(other.transform.Find("Camera").gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _currentEnemy = null;
            _enemies.Remove(other.gameObject);
            var enemyCam = other.transform.Find("Camera").gameObject;

            if (_enemies.Count <= 1 && enemyCam.activeInHierarchy)
            {
                ToggleCamera(enemyCam);
                ToggleCamera(_mainCamera);
            }
        }
    }

    private void HandleMultipleEnemies()
    {
        if (_enemies.Count > 1)
        {
            var _minDistance = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                // Disable any active enemy camera
                enemy.transform.Find("Camera").gameObject.SetActive(false);

                // Measure the distance between the enemy and the closest point on the center view
                float enemyDistance = CalculateClosestPosition(enemy.transform.position);

                if (IsDebug)
                    Debug.DrawLine(enemy.transform.position,
                        _centerView.GetComponent<SightJackCenter>()
                            .MeshCollider
                            .ClosestPoint(enemy.transform.position), Color.red);

                if (enemyDistance < _minDistance)
                {
                    // If it's closer than the others
                    if (_targetEnemy != null) // Turn off the previous enemy's camera
                        _targetEnemy.transform.Find("Camera").gameObject.SetActive(false);
                    _minDistance = enemyDistance; // Record the distance                
                    _targetEnemy = enemy; // Record the new enemy
                }
            }
            
            // Activate the closest enemy camera
            _targetEnemy.transform.Find("Camera").gameObject.SetActive(true);
        }
    }

    private float CalculateClosestPosition(Vector3 position)
    {
        return Vector3.Distance(position,
            _centerView.GetComponent<SightJackCenter>()
                .MeshCollider
                .ClosestPoint(position));
    }

    private void OnDestroy()
    {
        _enemies.ForEach(e => e.transform.Find("Camera").gameObject.SetActive(false));
        
        _mainCamera.SetActive(true);
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
            camera.GetComponent<StudioListener>().enabled = false;
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
            camera.GetComponent<StudioListener>().enabled = true;
        }
    }

    private void BalanceNoise()
    {
        _uiCamera.enabled = true;

        if (_enemies.Count > 1 && _targetEnemy != null)
        {
            // Multiple enemies = _targetEnemy
            float enemyDistance = CalculateClosestPosition(_targetEnemy.transform.position);

            SetNoise(0 + enemyDistance * 80, 0 + enemyDistance * 4);
        }
        else if (_currentEnemy != null)
        {
            // Single enemy = _currentEnemy
            float enemyDistance = CalculateClosestPosition(_currentEnemy.transform.position);

            SetNoise(0 + enemyDistance * 80, 0 + enemyDistance * 4);
        }
        else
        {
            // Default values = NULL
            SetNoise(20, 1);
        }
    }

    private void SetNoise(float strength, float alpha)
    {
        _noise.GetComponent<Image>().material.SetFloat("_Strength", strength);
        _noise.GetComponent<Image>().material.SetFloat("_Alpha", Mathf.Clamp(alpha, 0, 1));
    }

    private void AutomateSounds()
    {
        var controllerAxis = Mathf.Abs(Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"));
        var alpha = _noise.GetComponent<Image>().material.GetFloat("_Alpha");

        _noiseEvent.setParameterValue("SearchingForJacking", alpha);
        _noiseEvent.setParameterValue("radioNoise", controllerAxis);
    }
}
