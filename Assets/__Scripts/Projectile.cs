using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	[SerializeField]
	private WeaponType _type;
	public WeaponType type {
		get {
			return(_type);
		}
		set {
			SetType (value);
		}
	}
	// Use this for initialization
	void Awake () {
		InvokeRepeating ("CheckOffScreen", 2f, 2f);
	}
	public void SetType(WeaponType eType){
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		renderer.material.color = def.projectileColor;
	}
	void CheckOffScreen(){
		if (Utils.ScreenBoundsCheck (collider.bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
