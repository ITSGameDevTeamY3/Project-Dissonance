using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class SightJackController : MonoBehaviour
{
    private SightJackRays _sightJackVision;
    private ThirdPersonUserControl _thirdPersonUserControl;

	// Update is called once per frame
	void Update ()
	{
	    Listen();
	}

    private void Listen()
    {
        if (Input.GetKeyDown(KeyCode.E) || CrossPlatformInputManager.GetButtonDown("XBOX_LEFT_BUMPER"))
        {
            _thirdPersonUserControl = GetComponent<ThirdPersonUserControl>();

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
        _sightJackVision = gameObject.AddComponent<SightJackRays>();
    }

    public void Deactivate()
    {
        Destroy(_sightJackVision);
        _thirdPersonUserControl.m_Disabled = false;
    }
}
