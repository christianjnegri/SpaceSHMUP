using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero		S;
	public float gameRestartDelay = 2f;
	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;
	[SerializeField]
	private float	_shieldLevel=1;
	public bool	_____________________;
	public Bounds bounds;
	public delegate void WeaponFireDelegate();
	public WeaponFireDelegate fireDelegate;
	void Awake(){
		S = this;
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		bounds.center = transform.position;
		
		// constrain to screen
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.onScreen);
		if (off != Vector3.zero) {  // we need to move ship back on screen
			pos -= off;
			transform.position = pos;
		}
		
		// rotate the ship to make it feel more dynamic
		transform.rotation =Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);
		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) {
			Debug.Log("Jump");
			fireDelegate ();
		}
	}
	public GameObject lastTriggerGo = null;
	void OnTriggerEnter(Collider other) {
		GameObject go = Utils.FindTaggedParent (other.gameObject);
		if (go != null) {
			if (go == lastTriggerGo){
				return;
			}
			lastTriggerGo = go;
			if(go.tag == "Enemy"){
				shieldLevel--;
				Destroy(go);
			}else{
				print ("Triggered: " + go.name);
			}
		} else {
			print ("Triggered: " + other.gameObject.name);
		}
	}
	public float shieldLevel {
		get {
			return(_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			if (value < 0) {
				Destroy (this.gameObject);
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}
}