using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class SightJackController : MonoBehaviour
{
    public int Range = 10;
    public int FieldOfView = 15;
    public int Height = 100;
    public int Delay = 2;

    public Material Frustum;
    public Material Center;

    public Animator Fade;

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

            if (_thirdPersonUserControl != null)
            {
                if (!_thirdPersonUserControl.m_Disabled)
                {
                    StartCoroutine(Activate());
                }
                else
                {
                    Deactivate();
                }
            }
        }
    }

    private IEnumerator Activate()
    {
        _thirdPersonUserControl.m_Disabled = true;
        Fade.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.1f);
        _sightJackView = gameObject.AddComponent<SightJackView>();
    }

    public void Deactivate()
    {
        Destroy(_sightJackView);
        Fade.SetTrigger("FadeOut");
        _thirdPersonUserControl.m_Disabled = false;
    }
}
