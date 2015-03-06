using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class RunSimulation : MonoBehaviour {

	//main y = -159


	// Implementation of dictionary that resets the position for an object to its origin
	public Dictionary<string, Vector3> resetPosition = new Dictionary <string, Vector3>();

	bool finished = false;

	private string[] objects = 
	{
		"bread",  			// 0
		"cutting_board", 	// 1
		"bread_knife", 		// 2
		"bowl", 			// 3
		"garlic", 			// 4
		"butter", 			// 5
		"parsley", 			// 6
		"fork", 			// 7	
		"garlic_presser", 	// 8
		"salt", 			// 9
		"salt_grinder", 	// 10
		"butter_knife",     // 11
		"baking_pan/tray",  // 12
		"plate"				// 13
		
		/*
		,
		 // "garlic_salt", 
		"oven",  14
		"spoon",  15
		"aluminum_foil",
		"pepper_shaker",
		"eggs",
		"cheese",
		"cup",
		"teacup",
		"tea_bag",
		"kettle",
		"stove",
		"saucepan",
		"paper_filter",
		"milk",
		"sugar",
		"jar",
		"teapot",
		"water_faucet",
		"water_filter"
		*/
		
		
	}; 
	// I only have it like this to make it easy to compare to list.

	// Object states
	private string[][] states = 
	{ 
		new string[]{"whole", "half", "buttered", "baked"},
		new string[]{""},
		new string[]{""},
		new string[]{"empty", "garlic", "butter", "parsley", "butter+garlic", "butter+garlic+parsley", "butter+garlic+parsley+salt", "mixed_butter"},
		new string[]{"complete_clove", "chopped"},
		new string[]{"80g_room_temperature", "mixed"},
		new string[]{"finely chopped"},
		new string[]{"clean", "dirty"},
		new string[]{"clean", "open", "closed", "garlic", "empty"},
		new string[]{""},
		new string[]{""},
		new string[]{"clean", "unclean"},
		new string[]{""},
		new string[]{""}
		
		/*
		,
		new string[]{""}, 
		new string[]{"on", "off"}, 
		new string[]{"clean", "dirty"}, 
		new string[]{"flat", "folded", "unfolded"},
		new string[]{""},
		new string[]{"whole", "unshelled", "beaten"},
		new string[]{"whole", "shredded", "melted"},
		new string[]{"empty", "contains"},
		new string[]{"empty", "sugar", "milk", "sugar+milk", "tea"},
		new string[]{""},
		new string[]{"empty", "room_temp_water", "hot_water"},
		new string[]{"on", "off"},
		new string[]{"water", "water+tea_leaves", "oil"},
		new string[]{'clean', "with_leaves"},
		new string[]{""},
		new string[]{""},
		new string[]{"open+teabag", "closed+teabag"},
		new string[]{"empty+closed", "empty+open", "open+teabag", "closed+teabag", "open+teabag+water", "closed+teabag+water", "tea"},
		new string[]{"on", "off"},
		new string[]{""}

		*/
	};
	
	
	private string[] motions = 
	{
		"pick+place", "cut", "close tool", "hold", "brush off", 
		"pour", "mix", "spread", "scrape", "grind", "scoop"
		/*
		,
		"grind", "bake","cool/sit","wrap",
		"unwrap","switch/turn_on","dip*","wait","open_tool/object"
		/*/
		
	};
	
	public string[] description = { "<in motion>", "<not in motion>", "<assumed>" };

	//

	private Queue contents = new Queue();
	private List<string> networkList = new List<string>();


	//start and update functions
	
	
	// Use this for initialization
	void Start () {
	

		Debug.Log (DecodeNames ("O_10_1\tM_9\t1"));

		ReadFile ("Assets/garlicbreadv1");
		GetNextMotion();
		StartCoroutine (WaitSeconds (5));

	}
	
	// Update is called once per frame
	void Update () {
		//Pick and Place Motion between bread (0) and cutting board (1).
		PickMotions (motions, GameObject.Find (objects[0]), GameObject.Find (objects[1])); 

		// Hold Motion for knife (2).
		PickMotionsHold (motions, GameObject.Find (objects [2]));

		// Grind Motion for salt grinder (10).
		PickMotionsGrind (motions, GameObject.Find (objects [10]));

		//
				
		//PickMotions (motions, GameObject.Find (objects [2]));
		// PickMotions (motions, GameObject.Find (objects [2]), GameObject.Find (objects [0]));

		//GameObject.Find("bread").transform.position = Vector3.Lerp (GameObject.Find("bread").transform.position, GameObject.Find("cutting_board").transform.position, 0.01f);

	}



	void ReadFile(string fileName)
	{
		StreamReader readFile = new StreamReader(fileName + ".txt");
		string line = "";
		while ((line = readFile.ReadLine()) != null)
		{
			if(line[0] == 'O')
			{
				networkList.Add(line);
				contents.Enqueue(line);
			}
		}
	}
	
	void GetNextMotion()
	{
		List<string> nextNode = new List<string> ();

		nextNode.Add ((string)contents.Dequeue ());
		nextNode.Add ((string)contents.Dequeue ());

		foreach(string s in nextNode)
		{
			Debug.Log("s = " + s);
		}

		string[] aNames = DecodeNames (nextNode [0]);
	//  string[] bNames = DecodeNames (nextNode [1]);

		foreach(string s in aNames)
		{
			Debug.Log("= " + s);
		}
		
	}

	// Resets the posiiton of the object to origin
	void ResetPosition()
	{
		Debug.Log ("ONE!!!!!!!!!!!!!!!!!!!!!!!");
		// Add reset poition for each object which is not involved in multiple motions

		// Reset position for knife when it is placed back on the counter after cut motion is performed
		Vector3 originOfKnife = new Vector3 (-174.08f, -163.17f, 180.28f);  
	//	resetPosition.Add(objects[2], originOfKnife);
		GameObject.Find (objects [2]).transform.position = originOfKnife;

		if(GameObject.Find (objects [2]).transform.position == originOfKnife)
		{
			Debug.Log("stuff");
			finished = false;
		}
		 
		Debug.Log("____ResetPosition__");
	
	}
	// PickMotions for one object
	void PickMotionsHold (string[] networkNode, GameObject main)
	{
		//Hold motion
		StartCoroutine (Hold (networkNode, main));
	}

	void PickMotionsGrind (string[] networkNode, GameObject main)
	{
		// Grind Motion
		StartCoroutine (Grind (networkNode, main));

	}

	//TODO add motion blocks and test
	// PickMotions for two objects
	void PickMotions(string[] networkNode, GameObject main, GameObject other)
	{
	//	string j = networkNode[2].ToLower ();
		//Debug.Log (j);

		//pick+place
		StartCoroutine (PickandPlace (networkNode, main, other));
			//cut
		//StartCoroutine (Cut (networkNode, main, other));

	


		//main.transform.position = Vector3.Lerp (main.transform.position, flatMidPoint(main, other), 0.01f);
		//Debug.Log ("finished transforming position");
		//WaitSeconds (2);
		//main.transform.position = Vector3.Lerp (main.transform.position, flatEndPoint (main, other), 0.01f);
		/*
		switch(j)
		{
		case motions[0]:
			break;
		case motions[1]:
			break;
		case motions[2]:
			break;
		case motions[3]:
			break;
		case motions[4]:
			break;
		case motions[5]:
			break;
		case motions[6]:
			break;
		case motions[7]:
			break;
		case motions[8]:
			break;
		case motions[9]:
			break;
		case motions[10]:
			break;
		}
		*/
	}



	/// <summary>
	/// Decodes the names.
	/// </summary>
	/// <returns>The names.</returns>
	/// <param name="g">The gameobject component.</param>
	string[] DecodeNames(string g)
	{
		string decodedName = "";
		
		string[] selections = g.Split ('\t');
		
		if(g[0] == 'O')
		{
			string[] stateNames = selections[0].Split('_');
			string[] motionNames = selections[1].Split('_');

			int objIndex = int.Parse(stateNames[1]) - 1;   // gets last element of an array
			int stateIndex = int.Parse(stateNames[2]) - 1;
			int motionIndex = int.Parse(motionNames[1]) - 1;
			int deIndex = int.Parse (selections [2]);
			//Debug.Log("!!!" + objIndex);
			
			if(objIndex < 0 || (objIndex > objects.Length -1))
				objIndex = 0;
			if(stateIndex < 0 || (stateIndex > states[objIndex].Length -1))
				stateIndex = 0;
			if(motionIndex < 0 || (motionIndex > motions.Length -1))
				motionIndex = 0;
			
			decodedName += (objects[objIndex] + "\t" + 
			                states[objIndex][stateIndex] + "\t" + 
			                motions[motionIndex] + "\t" +
			                description[deIndex]);
		}
		
		if(g[0] == 'M')
		{
			string[] stateNames = selections[1].Split('_');
			string[] motionNames = selections[0].Split('_');
			
			int objIndex = int.Parse(stateNames[1]) - 1;
			int stateIndex = int.Parse(stateNames[2]) - 1;
			int motionIndex = int.Parse(motionNames[1]) - 1;
			int deIndex = int.Parse (selections [2]);

			if(objIndex < 0)
				objIndex = 0;
			if(stateIndex < 0)
				stateIndex = 0;
			if(motionIndex < 0)
				motionIndex = 0;

		
			decodedName += (motions[motionIndex] + "\t" + 
			                objects[objIndex] + "\t" + 
			                states[objIndex][stateIndex] + "\t" + 
			                description[deIndex]);
		}
		
			string[] returnNames = decodedName.Split ('\t');

		return returnNames;
		
	}

	Vector3 flatMidPoint(GameObject one, GameObject two)
	{

		float ptx = (one.transform.position.x + two.transform.position.x) / 2;
		float pty = -162f;
		float ptz = (one.transform.position.z + two.transform.position.z) / 2;

		Vector3 pt = new Vector3 (ptx, pty, ptz);
		return pt;
	}

	Vector3 flatEndPoint(GameObject one, GameObject two)
	{


		float ptx = two.transform.position.x;
		float pty = two.transform.position.y + 0.05f;
		float ptz = two.transform.position.z;
		Vector3 pt = new Vector3 (ptx, pty, ptz);
		return pt;
	}

	Vector3 verticalPoint(GameObject one)
	{
		float ptx = one.transform.position.x;
		float pty = -160.5f;
		float ptz = one.transform.position.z;

		Vector3 pt = new Vector3 (ptx, pty, ptz);
		return pt;
	}

	Vector3 grindMotionPosition(GameObject one)
	{
		float ptx = -175.85f;
		float pty = -161.3f;
		float ptz = 179f;

		Vector3 pt = new Vector3 (ptx, pty, ptz);
		return pt;
	 }
	 
		/// <summary>
		/// Waits the seconds.
		/// </summary>
		/// <returns>The seconds.</returns>
		/// <param name="seconds">Seconds.</param>
		/// 
		/// 

	IEnumerator PickandPlace(string[] networkNode, GameObject main, GameObject other)
	{
		Vector3 tmp = flatMidPoint (main, other);
		if( ((int)main.transform.position.x != (int)tmp.x) || ((int)main.transform.position.z != (int)tmp.z) )
		{
			main.transform.position = Vector3.Slerp( main.transform.position, tmp, 0.05f);
			//Debug.Log((int)main.transform.position.x + " " + (int)tmp.x + " ___ " + (int)main.transform.position.z + " " + (int)tmp.z);
		}
		else
		{
//			Debug.Log("bread can drop now");
			Rigidbody r = main.GetComponent<Rigidbody> ();			
			r.useGravity = true;
		}

		yield return new WaitForSeconds(2);
	}

	IEnumerator Cut(string[] networkNode, GameObject main, GameObject other)
	{

	//	Vector3 tmp = flatMidPoint (main, other);

	//	if( ((int)main.transform.position.x != (int)tmp.x) || ((int)main.transform.position.z != (int)tmp.z) )
	//	{
	//		main.transform.position = Vector3.Slerp( main.transform.position, tmp, 0.05f);
			//Debug.Log((int)main.transform.position.x + " " + (int)tmp.x + " ___ " + (int)main.transform.position.z + " " + (int)tmp.z);
	//	}
	//	else
	//	{
	//		Vector3[] vertices = other.GetComponent<MeshFilter>().mesh.vertices;


	//	}

		yield return new WaitForSeconds (2);
	}


	//"pick+place", "cut", "close tool", "hold", "brush off", 
	//"pour", "mix", "spread", "scrape", "grind", "scoop"
	IEnumerator CloseTool(string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}

	IEnumerator Hold(string[] networkNode, GameObject main)
	{
				yield return new WaitForSeconds (3f);

				Vector3 temp = verticalPoint (main);
				Debug.Log ("Main position x: " + (int)main.transform.position.x + " Temp position: " + (int)temp.x + "");
				Debug.Log ("Main position y: " + (int)main.transform.position.y + " Temp position: " + (int)temp.y + "");
				Debug.Log ("Main position z: " + (int)main.transform.position.z + " Temp position: " + (int)temp.z + "");

				if (finished == false) {

						if (((int)temp.y) != ((int)160.5f)) // -160.5f
						{
								main.transform.position = Vector3.Lerp (main.transform.position, temp, 0.05f); 
								Debug.Log ((int)main.transform.position.y + " " + (int)temp.y + "____");

						}
						
						if (finished) {
								ResetPosition ();
								Debug.Log ("NOT FINISHED");
						}

						yield return new WaitForSeconds (2);
				}
				
		}

	IEnumerator BrushOff(  string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}
	
	IEnumerator Pour(string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}

	IEnumerator Mix(string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}
	
	IEnumerator Spread(string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}


	/// <summary>
	/// use the fork or equivalent as the main object to scrape items off of other object
	/// </summary>
	/// <param name="networkNode">Network node.</param>
	/// <param name="main">Main.</param>
	/// <param name="other">Other.</param>
	IEnumerator Scrape(string[] networkNode, GameObject main, GameObject other)
	{
		
		yield return new WaitForSeconds (2);
	}
	
	IEnumerator Grind(string[] networkNode, GameObject main)
	{
		yield return new WaitForSeconds (6f);
		float rtx = main.transform.rotation.x;
		float rty = 30f;
		float rtz = 119.7f;
		float rtw = -150f;

		Vector3 temp = grindMotionPosition (main);
		Quaternion rotation = new Quaternion (rtx, rty, rtz, rtw);
    //	Debug.Log ("Main position x: " + (int)main.transform.position.x + " Temp position: " + (int)temp.x + "");
	//	Debug.Log ("Main position y: " + (int)main.transform.position.y + " Temp position: " + (int)temp.y + "");
	//	Debug.Log ("Main position z: " + (int)main.transform.position.z + " Temp position: " + (int)temp.z + "");


		if (((int)temp.y) != ((int)161.3f)) 
		{
			main.transform.position = Vector3.Slerp (main.transform.position, temp, 0.05f); 
		//	Debug.Log ((int)main.transform.position.y + "GRIND" + " " + (int)temp.y + "____");
		}
			
		if((((int)main.transform.position.y) == ((int)-161.3f)) && (((int)main.transform.position.z) == ((int)179f)))
		{
			main.transform.rotation = Quaternion.Lerp(main.transform.rotation, rotation, 0.05f);
			Debug.Log ("THE POSITION OF Y IS EQUAL TO 161.3f");
		}
		yield return new WaitForSeconds (2);
	}


	IEnumerator Scoop(string[] networkNode, GameObject main, GameObject other)
	{

		yield return new WaitForSeconds (2);
	}

	/// <summary>
	/// Waits the seconds.
	/// </summary>
	/// <returns>The seconds.</returns>
	/// <param name="seconds">Seconds.</param>
	IEnumerator WaitSeconds(int seconds)
	{

		yield return new WaitForSeconds(seconds);
	}
}
