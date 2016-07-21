using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public class Main : MonoBehaviour {
	static public Main S ;
	static public Dictionary<WeaponType,WeaponDefinition> W_DEFS;
	static public int SCORE;
	static public int SCORE_FROM_PREV_ROUND = 0;
	static public int HIGH_SCORE = 0;
	public GameObject[] prefabEnemies;
	public float enemySpawnPadding = 1.5f ;
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public GameObject GOStartText;
	public WeaponType[]          powerUpFrequency = new WeaponType[] {
		WeaponType.blaster, WeaponType.blaster, WeaponType.shield, WeaponType.blaster, 
		WeaponType.spread, WeaponType.spread, WeaponType.shield, WeaponType.spread, 
		WeaponType.shield};
	public bool _______________ ;
	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;
	public static int wave = 0;
	public static int amountDestroyed = 0;
	static public bool gameOn = false;
	public GUIText GTWaveOver;
	public GUIText GTWave;
	public GUIText GTScore;
	public GUIText GTAmountLeft;
	public int amountPerWave = 0;
	public bool gameOver = false;

	void Awake () {
		S = this;
		//Set Utils.cambounds
		Utils.SetCameraBounds (this.camera);
		GameObject goWO = GameObject.Find("WaveOver");
		if (goWO != null) { 
			GTWaveOver = goWO.GetComponent < GUIText >(); 
		}
		GTWaveOver.gameObject.SetActive(false);
		GameObject goW = GameObject.Find("Wave");
		if (goW != null) { 
			GTWave = goW.GetComponent < GUIText >(); 
		}
		GTWave.gameObject.SetActive(false);
		GameObject goS = GameObject.Find("Score");
		if (goS != null) { 
			GTScore = goS.GetComponent < GUIText >(); 
		}
		GameObject goAL = GameObject.Find("AmountLeft");
		if (goS != null) { 
			GTAmountLeft = goAL.GetComponent < GUIText >(); 
		}
		GTAmountLeft.gameObject.SetActive(false);
		W_DEFS = new Dictionary<WeaponType,WeaponDefinition> ();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS [def.type] = def;
		}
	}

	static public WeaponDefinition GetWeaponDefinition(WeaponType wt){
		if (W_DEFS.ContainsKey (wt)) {
			return(W_DEFS [wt]);
		}
		return(new WeaponDefinition ());
	}

	void Update(){
		if (Input.GetAxis ("Jump") == 1 && gameOver) {
			Debug.Log("Jump");
			Restart();
		}
	}
	void Start(){
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for(int i=0; i<weaponDefinitions.Length; i++){
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
		Screen.SetResolution (630, 900, false);
	}

	public void SpawnEnemy(){
		if(!(WaveOver())){
			Debug.Log("Spawn Called");
			//pick a random enemy prefab to instantiate
			int ndx = Random.Range (0,(prefabEnemies.Length-1));
			GameObject go = Instantiate (prefabEnemies[ndx])as GameObject ;
			//position the Enemy above the screen with a random x positions
			Vector3 pos = Vector3.zero;
			float xMin = Utils.camBounds.min.x + enemySpawnPadding;
			float xMax = Utils.camBounds.max.x - enemySpawnPadding;
			pos.x = Random.Range (xMin, xMax);
			pos.y = Utils.camBounds.max.y + enemySpawnPadding;
			go.transform.position = pos;
			Invoke ("SpawnEnemy", enemySpawnRate);
		}
	}
	public void ShipDestroyed( Enemy e ) {
		amountDestroyed++;
		SCORE += e.score;
		GTScore.text = "" + SCORE;
		if(WaveOver()){
			Parallax.scrollSpeed = -3;
			GTWave.gameObject.SetActive(false);
			GTAmountLeft.gameObject.SetActive(false);
			GTWaveOver.text = "WAVE " + wave + " OVER";
			GTWaveOver.gameObject.SetActive(true);
			Invoke ("NewWave", 4);
			GameObject[] tEnemyArray = GameObject.FindGameObjectsWithTag ("Enemy");
			foreach(GameObject tGO in tEnemyArray){
				Destroy(tGO);
			}		
		}else{
			if ((amountPerWave - amountDestroyed) > 1){
				GTAmountLeft.text = "DESTROY " + (amountPerWave - amountDestroyed) + " MORE SHIPS!";
			} else {
				GTAmountLeft.text = "DESTROY " + (amountPerWave - amountDestroyed) + " MORE SHIP!";
			}

			// Potentially generate a PowerUp
			if (Random.value <= e.powerUpDropChance) {
				// Random.value generates a value between 0 & 1 (though never == 1)
				// If the e.powerUpDropChance is 0.50f, a PowerUp will be generated
				//   50% of the time. For testing, it's now set to 1f.
				
				// Choose which PowerUp to pick
				// Pick one from the possibilities in powerUpFrequency
				int ndx;
				if (wave < 20){
					ndx = Random.Range(0,(2+wave));
				}else{
					ndx = Random.Range(0,(20));
				}
				WeaponType puType = powerUpFrequency[ndx];
				
				// Spawn a PowerUp
				GameObject go = Instantiate( prefabPowerUp ) as GameObject;
				PowerUp pu = go.GetComponent<PowerUp>();
				// Set it to the proper WeaponType
				pu.SetType( puType );
				
				// Set it to the position of the destroyed ship
				pu.transform.position = e.transform.position;
			}
		}
	}
	public void NewWave(){
		wave++;
		Parallax.scrollSpeed = -20f - (wave * 5);
		GTWaveOver.gameObject.SetActive(false);
		GTWave.text = "WAVE " + wave;
		GTWave.gameObject.SetActive(true);
		
		amountPerWave = (wave*5);
		amountDestroyed = 0;
		enemySpawnRate = (3f/(wave+1))+1f;
		GTAmountLeft.text = "DESTROY " + amountPerWave + " MORE SHIPS!";
		GTAmountLeft.gameObject.SetActive(true);
		
		//Invoke call spawnenemy() once after a 2 second delay
		
		Invoke ("SpawnEnemy", enemySpawnRate);
		
	}
	public void Bomb(){
		GameObject[] tEnemyArray = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach(GameObject tGO in tEnemyArray){
			Destroy(tGO);
		}
		if((amountPerWave - amountDestroyed) - (tEnemyArray.Length) < 1){
			amountDestroyed = amountPerWave;
			Parallax.scrollSpeed = -3;
			GTWave.gameObject.SetActive(false);
			GTWaveOver.text = "WAVE " + wave + " OVER";
			GTWaveOver.gameObject.SetActive(true);
			Invoke ("NewWave", 4);
		} else if ((amountPerWave - amountDestroyed) > 1){
			amountDestroyed -= tEnemyArray.Length;
			GTAmountLeft.text = "DESTROY " + (amountPerWave - amountDestroyed) + " MORE SHIPS!";
		} else {
			amountDestroyed -= tEnemyArray.Length;
			GTAmountLeft.text = "DESTROY " + (amountPerWave - amountDestroyed) + " MORE SHIP!";
		}
	}
	public bool WaveOver(){
		bool over = false;
		if(amountDestroyed == amountPerWave){
			over = true;
		}
		return over;
	}
	public void GameOver(float delay){
		GTAmountLeft.gameObject.SetActive(false);
		Parallax.scrollSpeed = -3f;
		GTWave.gameObject.SetActive(false);
		GTWaveOver.text = "GAME OVER";
		GTWaveOver.gameObject.SetActive(true);
		Invoke ("ShowResults", delay);
		gameOver = true;
	}
	public void ShowResults(){
		string results = "WAVE " + wave +"\n" + SCORE;
		if(HIGH_SCORE < SCORE){
			HIGH_SCORE = SCORE;
			results += "\n\n NEW HIGH\n SCORE!";	
		}
		GTWaveOver.text = results;
	}
	public void Restart(){
		gameOver=false;
		wave = 0;
		SCORE = 0;
		Application.LoadLevel ("_Scene_1");
				
	}
}
