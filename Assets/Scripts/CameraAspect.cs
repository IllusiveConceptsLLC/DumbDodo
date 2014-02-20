using UnityEngine;
using System.Collections;

public class CameraAspect : MonoBehaviour {
	void Awake () {
		camera.orthographicSize = Screen.height / 2;
	}
}
