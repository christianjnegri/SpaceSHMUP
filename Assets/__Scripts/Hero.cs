using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero		S;
	public float gameRestartDelay = 2f;
	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;
	[SerializeField]
	private float	_shieldLevel=0;
	public Color[] originalColors;
	public Weapon[]            weapons;
	public bool	_____________________;
	public Material[] materials;
	public Bounds bounds;
	public delegate void WeaponFireDelegate();
	public WeaponFireDelegate fireDelegate;
	void Awake(){
		
		S = this;
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i=0; i<materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
	}


	// Use this for initialization
	void Start () {
		ClearWeapons();
		weapons[0].SetType(WeaponType.blaster);
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
				speed=25;
				for (int i=0; i<materials.Length; i++) {
					materials [i].color = originalColors [i];
				}
				Destroy(go);
				
			} else if (go.tag == "PowerUp") {
			// If the shield was triggerd by a PowerUp
			AbsorbPowerUp(go);
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
	
				Main.S.GameOver(gameRestartDelay);
			}
		}
	}
	public void AbsorbPowerUp( GameObject go ) {
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield: // If it's the shield
			shieldLevel++;
			break;

		case WeaponType.bomb:
			Main.S.Bomb ();
		break;
			
		case WeaponType.speed: //if it's speed
			speed=50;
			foreach (Material m in materials) {
				m.color = Color.yellow;
			}
			break;
		
		default: // If it's any Weapon PowerUp
			// Check the current weapon type
			if (pu.type == weapons[0].type) {
				// then increase the number of weapons of this type
				Weapon w = GetEmptyWeaponSlot(); // Find an available weapon
				if (w != null) {
					// Set it to pu.type
					w.SetType(pu.type);
				}
			} else {
				// If this is a different weapon
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy( this.gameObject );
	}
	
	Weapon GetEmptyWeaponSlot() {
		for (int i=0; i<weapons.Length; i++) {
			if ( weapons[i].type == WeaponType.none ) {
				return( weapons[i] );
			}
		}
		return( null );
	}
	
	void ClearWeapons() {
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}
}