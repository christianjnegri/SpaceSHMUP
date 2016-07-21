using UnityEngine;
using System.Collections;

public class StartText : MonoBehaviour {
	public float speed = .5f;
	public float health = 5;
	public int showDamageForFrames = 2;
	public bool ____________________;
	public Color[] originalColors;
	public Material[] materials;
	public int remainingDamageFrames = 0;
	public Bounds bounds;
	public Vector3 boundsCenterOffset;
	
	// Use this for initialization
	void Awake () {
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i=0; i<materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if (remainingDamageFrames == 0) {
				UnShowDamage ();
			}
		}
	}
	public virtual void Move(){
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
		if(pos.y <0 || pos.y > 20){
			speed*=(-1);
		}
	}
	public Vector3 pos {
		get {
			return(this.transform.position);
		}
		set {
			this.transform.position = value;
		}
	}
	void CheckOffScreen() {
		if (bounds.size == Vector3.zero) {
			bounds = Utils.CombineBoundsOfChildren (this.gameObject);
			boundsCenterOffset = bounds.center - transform.position; 
		}
		bounds.center = transform.position + boundsCenterOffset;
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			if (off.y < 0) {
				Destroy (this.gameObject);
			}
		}
	}
	void OnCollisionEnter(Collision coll){
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile> ();
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy (other);
				break;
			}
			ShowDamage();
			health -= Main.W_DEFS [p.type].damageOnHit;
			if (health <= 0) {
				Destroy (this.gameObject);
				Destroy(other);
				Debug.Log ("START");
				Main.S.NewWave();
			}
			Destroy(other);
			break;
		}
	}
	public void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;
		}
		remainingDamageFrames = showDamageForFrames;
	}
	public void UnShowDamage() {
		for (int i=0; i<materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
	}
}
