//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
//public class EnterSound : MonoBehaviour {

//    public AudioClip enterSound;

//    private Button button { get{ return GetComponent<Button>(); } }
//    private AudioSource audioSource { get { return GetComponent<AudioSource>(); } }

//	// Use this for initialization
//	void Start () {
//        gameObject.AddComponent<AudioSource>();
//        audioSource.clip = enterSound;
//        audioSource.playOnAwake = false;

//        button.onClick.AddListener(() => PlaySound());
//	}
	
//	// Update is called once per frame
//	void PlaySound()
//    {
//        audioSource.PlayOneShot(enterSound);
//    }
//}
