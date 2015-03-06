using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TrackObject : MonoBehaviour {

	private bool tracking = false;
	private bool animating = false;
	private List<Vector3> marks = new List<Vector3>();

	private List<Vector3> movePoints = new List<Vector3>();



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyUp(KeyCode.T))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if(Input.GetKeyUp(KeyCode.Q))
		{
			tracking = !tracking;
			Debug.Log(tracking);
		}

		RecordPoints (GameObject.FindGameObjectWithTag ("cube"));
		ReadPoints (GameObject.FindGameObjectWithTag ("cube"));
	
	}

	void RecordPoints(GameObject g)
	{
		if(tracking)
		{
			if(Input.GetKeyUp(KeyCode.W))
			{
				//begin tracking
				marks.Add(g.transform.position);
				marks.Add(g.transform.rotation.eulerAngles);

				Debug.Log(marks[marks.Count-1]);
			}

		}

		if(Input.GetKeyUp(KeyCode.E))
		{
			using (StreamWriter writer = new StreamWriter("Assets/cubePoints.txt"))
			{

				for(int i=0; i<marks.Count; ++i)
				{
					if(i%2 == 0)
					{
						writer.Write(marks[i].x + " " + marks[i].y + " " + marks[i].z + " ");
					}
					else
					{
						writer.WriteLine(marks[i].x + " " + marks[i].y + " " + marks[i].z);
						writer.WriteLine();
					}
				}

				foreach(Vector3 v in marks)
				{
					writer.WriteLine(v.x + " " + v.y + " " + v.z);
					Debug.Log("-> " + v);
				}
			}
		}

	}

	void ReadPoints(GameObject g)
	{
		if(Input.GetKeyUp(KeyCode.R))
		{
			animating = !animating;
		}

		if(animating)
		{

			g.rigidbody.useGravity = false;
			using (StreamReader reader = new StreamReader("Assets/cubePoints.txt"))
			{
				string line;
				movePoints.Add(g.transform.position);
				while((line = reader.ReadLine()) != null)
				{
					string[] v3 = line.Split(' ');
					Vector3 moving = new Vector3();
					float xmov = float.Parse(v3[0]);
					float ymov = float.Parse(v3[1]);
					float zmov = float.Parse(v3[2]);

					moving.x = xmov;
					moving.y = ymov;
					moving.z = zmov;

					movePoints.Add(moving);
				
				}
			}

			//here
			int i = 0;
			while(i<(movePoints.Count-1))
			{
				g.transform.position = Vector3.Lerp(g.transform.position, movePoints[i], 0.01f);
				if(g.transform.position == movePoints[i])
				{
					i++;
				}
			}

			//g.transform.position = Vector3.Lerp(g.transform.position, movePoints[movePoints.Count-1], 0.01f);
			//g.transform.position = Vector3.Lerp(g.transform.position, movePoints[movePoints.Count-1], 0.01f);

			/*
			for(int i=0; i<movePoints.Count-1; ++i)
			{

				g.transform.position = Vector3.Lerp(movePoints[i], movePoints[i+1], 0.001f);
			}

			*/
		}
	}
}
