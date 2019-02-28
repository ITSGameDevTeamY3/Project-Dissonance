using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class SightJackController : MonoBehaviour
{
    public int Range = 10;
    public int FieldOfView = 15;
    public int Height = 100;
    public Material Frustum;
    public Material Center;

    [Header("Debug Options")]
    public bool IsDebug;
    public bool Scale;

    private SightJackView _sightJackView;
    private ThirdPersonUserControl _thirdPersonUserControl;

	void Update()
	{
	    Listen();
	}

    private void Listen()
    {
        if (Input.GetKeyDown(KeyCode.E) || CrossPlatformInputManager.GetButtonDown("XBOX_LEFT_BUMPER"))
        {
            _thirdPersonUserControl = GetComponentInParent<ThirdPersonUserControl>();

            if (_thirdPersonUserControl == null) return;

            if (!_thirdPersonUserControl.m_Disabled)
            {
                
                Activate();
            }
            else
            {
                Deactivate();
                
            }
        }
    }

    private void Activate()
    {
        _thirdPersonUserControl.m_Disabled = true;
        _sightJackView = gameObject.AddComponent<SightJackView>();
    }

    public void Deactivate()
    {
        Destroy(_sightJackView);
        _thirdPersonUserControl.m_Disabled = false;
    }
}
