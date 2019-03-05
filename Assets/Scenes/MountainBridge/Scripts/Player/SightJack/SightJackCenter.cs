using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SightJackCenter : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public MeshCollider MeshCollider;
    private Mesh _view;

    private SightJackView _parent;

    private void Start()
    {
        _parent = GetComponentInParent<SightJackView>();

        CreateView();
    }

    private void CreateView()
    {
        _view = _parent.View;

        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.receiveShadows = false;
        _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        _meshRenderer.material = _parent.CenterMaterial;

        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshFilter.mesh = _view;

        MeshCollider = gameObject.AddComponent<MeshCollider>();
        MeshCollider.sharedMesh = _view;
        MeshCollider.convex = true;
        MeshCollider.isTrigger = true;

        transform.localScale = new Vector3(0, 1, 1);
    }
	
	private void Update ()
	{
	    if (_parent.IsDebug)
	        _meshRenderer.enabled = true;
	    else
	        _meshRenderer.enabled = false;
    }
}
