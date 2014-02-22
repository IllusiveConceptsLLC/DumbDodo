using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer)), 
 RequireComponent(typeof(MeshFilter)),
 RequireComponent(typeof(MeshRenderer))
 ]
public class MeshTest : MonoBehaviour {
	public Texture texture;

	//Bezier bez;

	void Start()
	{
		GetComponent<MeshFilter>().sharedMesh = CreatePlaneMesh();
	}

	void Update() {
		//if (Input.GetMouseButtonDown (0)) {
		//}
		if (dirty) {
			GetComponent<MeshFilter>().sharedMesh = CreatePlaneMesh();
			dirty = false;
		}
	}

	Mesh CreatePlaneMesh() {
		return CreatePlaneMesh2();
	}

	// http://en.wikipedia.org/wiki/Sine_wave
	// y = amplitude * sin(freq * time + phase)
	// where:
	// A, the amplitude, is the peak deviation of the function from zero.
	// f, the ordinary frequency, is the number of oscillations (cycles) that occur each second of time.
	// ω = 2πf, the angular frequency, is the rate of change of the function argument in units of radians per second
	// φ, the phase, specifies (in radians) where in its cycle the oscillation is at t = 0.
	// When φ is non-zero, the entire waveform appears to be shifted in time by the amount φ/ω seconds. A negative value represents a delay, and a positive value represents an advance.

	float generatePoint (int t){
		//float amplitude = 100;
		//float angularFreq = hSliderValue;//.1f;//0.3f;
		//float phase = 250;
		float y = hAmplitudeValue * Mathf.Sin ( (hSliderValue * t) + hPhaseValue );
		return y;
	}

	float[] generateWave(int xPointSpacing) {
		// full width in pixels of this wave.
		int waveLength = 60*112; 

		// the number of points we will have in this wave.
		int pointCount = waveLength / xPointSpacing;

		// Array to hold all of the points.
		float[] yPointArray = new float[pointCount];

		for (int fix=0; fix<pointCount; fix++) {
			yPointArray[fix] = generatePoint(fix);
		}

		return yPointArray;
	}

	Mesh CreatePlaneMesh2() {
		Mesh mesh = new Mesh();

		/// we need to create 50 blocks
		int currentTriangle = 0;

		// The current block that we need to generate / draw
		int currentBlock = 0;

		// Offset between blocks, calculated at currentBlock*4
		int currentBlockOffset = 0;

		// width in pixels between wave points.
		int xPointSpacing = 20; 

		float[] yPoints = generateWave (xPointSpacing);

		int blocksToCreate = yPoints.Length/2;

		// There are 4 verticies per block
		Vector3[] vertices = new Vector3[blocksToCreate*4];

		// There are 4 UV's per block
		Vector2[] uv = new Vector2[blocksToCreate*4];

		// There are 6 triangles per block
		int[] triangles = new int[blocksToCreate*6];

		float textureWidth = 90; //60;
		float completePercent = 0;
		
		float partPerc1 = 0;//(1f / 33)*100;
		float partPerc2 = .11f; //.33f;//(1f / 33)*100;

		while (true) {

			if(completePercent==90){
				completePercent=0;
			}
			partPerc1 = (completePercent / textureWidth) ;
			completePercent+=10;
			partPerc2 = (completePercent / textureWidth) ;

			// Top left of block
			vertices[currentBlockOffset] = new Vector3(currentBlock*20, yPoints[currentBlock], 0);
			uv[currentBlockOffset] = new Vector2(partPerc1, 1);

			// Top Right of block
			vertices[currentBlockOffset+1] = new Vector3((currentBlock*20)+20, yPoints[currentBlock+1], 0);
			uv[currentBlockOffset+1] = new Vector2(partPerc2, 1);

			// Bottom Right of block
			vertices[currentBlockOffset+2] = new Vector3((currentBlock*20)+20, yPoints[currentBlock+1]-200, 0);
			uv[currentBlockOffset+2] = new Vector2(partPerc2, 0);

			// Bottom Left of block
			vertices[currentBlockOffset+3] = new Vector3(currentBlock*20, yPoints[currentBlock]-200, 0);
			uv[currentBlockOffset+3] = new Vector2(partPerc1, 0);

			triangles[currentTriangle++] = currentBlockOffset;
			triangles[currentTriangle++] = currentBlockOffset+2;
			triangles[currentTriangle++] = currentBlockOffset+3;
			triangles[currentTriangle++] = currentBlockOffset+0;
			triangles[currentTriangle++] = currentBlockOffset+1;
			triangles[currentTriangle++] = currentBlockOffset+2;

			if(currentTriangle >= triangles.Length){
				break;
			}

			currentBlock++;
			currentBlockOffset=currentBlock*4;
		}
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		renderer.sharedMaterial.mainTexture = texture;

		return mesh;
	}
 

	private float hSliderValue = .1f;
	private float hSliderValueOrig = .2f;

	private float hPhaseValue = 0.0f;
	private float hPhaseValueOrig = 0.0f;

	private float hAmplitudeValue = 100.0f;
	private float hAmplitudeValueOrig = 10.0f;

	private bool dirty = false;
	void OnGUI () {
		hSliderValueOrig = hSliderValue;
		GUI.Label (new Rect (25, 0, 300, 30), "Angular Frequency");
		hSliderValue = GUI.HorizontalSlider (new Rect (25, 25, 100, 30), hSliderValue, 0.0f, 1.0f);
		if (hSliderValueOrig != hSliderValue) {
			dirty = true;
		}

		hPhaseValueOrig = hPhaseValue;
		GUI.Label (new Rect (25, 50, 300, 30), "Phase");
		hPhaseValue = GUI.HorizontalSlider (new Rect (25, 75, 100, 30), hPhaseValue, 0.0f, 10.0f);
		if (hPhaseValueOrig != hPhaseValue) {
			dirty = true;
		}

		hAmplitudeValueOrig = hAmplitudeValue;
		GUI.Label (new Rect (25, 100, 300, 30), "Amplitude");
		hAmplitudeValue = GUI.HorizontalSlider (new Rect (25, 125, 100, 30), hAmplitudeValue, 0.0f, 1000.0f);
		if (hAmplitudeValueOrig != hAmplitudeValue) {
			dirty = true;
		}
	}
}

/*
 * 
	Vector3 CalculateBezierPoint(float t,
	                             Vector3 p0, 
	                             Vector3 p1, 
	                             Vector3 p2, 
	                             Vector3 p3){
		
		float u  = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0; //first term

		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term
		return p;
	}

	Mesh CreatePlaneMesh2() {
		Mesh mesh = new Mesh();

		/// we need to create 50 blocks
		int blocksToCreate = 50;
		int currentVertice = 0;
		int currentTriangle = 0;

		// There are 4 verticies per block
		Vector3[] vertices = new Vector3[blocksToCreate*4];

		// There are 4 UV's per block
		Vector2[] uv = new Vector2[blocksToCreate*4];

		// There are 6 triangles per block
		int[] triangles = new int[blocksToCreate*6];

		Vector3 vec1;
		Vector3 vec2;
		float amt;


		float textureWidth = 60;
		float completePercent = 0;

		float partPerc1 = 0;//(1f / 33)*100;
		float partPerc2 = .33f;//(1f / 33)*100;
		//Debug.Log ("p" + partPerc);


		for (int currentBlock=0; currentBlock<blocksToCreate; currentBlock++) {
			//Debug.Log (completePercent);
			if(completePercent==60){
				completePercent=0;
			}
			partPerc1 = (completePercent / textureWidth) ;
			completePercent+=20;
			partPerc2 = (completePercent / textureWidth) ;

			Debug.Log ("1:"+partPerc1+" 2:"+partPerc2);

			//vec1 = bez.GetPointAtTime(currentBlock);
			amt = ((float)currentBlock)/blocksToCreate;
			vec1 =	CalculateBezierPoint(amt, new Vector3(0,0,0), 
			                            new Vector3(10,200,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(blocksToCreate-1,0,0)
			                            );
			// Top left of block
			vertices[currentVertice] = new Vector3(currentBlock*20, vec1.y, 0);
			uv[currentVertice] = new Vector2(partPerc1, 1);
			currentVertice++;

			amt = ((float)currentBlock+1)/blocksToCreate;
			vec2 =	CalculateBezierPoint(amt, new Vector3(0,0,0), 
			                            new Vector3(10,200,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(blocksToCreate-1,0,0)
			                            );

			// Top Right of block
			vertices[currentVertice] = new Vector3((currentBlock*20)+20, vec2.y, 0);
			uv[currentVertice] = new Vector2(partPerc2, 1);
			currentVertice++;
			
			// Bottom Right of block
			vertices[currentVertice] = new Vector3((currentBlock*20)+20, vec2.y-50, 0);
			uv[currentVertice] = new Vector2(partPerc2, 0);
			currentVertice++;
			
			// Bottom Left of block
			vertices[currentVertice] = new Vector3(currentBlock*20, vec1.y-50, 0);
			uv[currentVertice] = new Vector2(partPerc1, 0);
			currentVertice++;

			triangles[currentTriangle++] = (currentBlock*4)+0;
			triangles[currentTriangle++] = (currentBlock*4)+2;
			triangles[currentTriangle++] = (currentBlock*4)+3;
			triangles[currentTriangle++] = (currentBlock*4)+0;
			triangles[currentTriangle++] = (currentBlock*4)+1;
			triangles[currentTriangle++] = (currentBlock*4)+2;
		}
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		renderer.sharedMaterial.mainTexture = texture;

		return mesh;
	}
 * 
 * 
	Mesh CreatePlaneMesh2() {
		Mesh mesh = new Mesh();

		/// we need to create 50 blocks
		int blocksToCreate = 500;
		int currentVertice = 0;
		int currentTriangle = 0;

		// There are 4 verticies per block
		Vector3[] vertices = new Vector3[blocksToCreate*4];

		// There are 4 UV's per block
		Vector2[] uv = new Vector2[blocksToCreate*4];

		// There are 6 triangles per block
		int[] triangles = new int[blocksToCreate*6];

		Vector3 vec1;
		Vector3 vec2;
		float amt;

		for (int currentBlock=0; currentBlock<blocksToCreate; currentBlock++) {

			//vec1 = bez.GetPointAtTime(currentBlock);
			amt = ((float)currentBlock)/blocksToCreate;
			vec1 =	CalculateBezierPoint(amt, new Vector3(0,0,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(blocksToCreate-1,0,0)
			                            );
			// Top left of block
			vertices[currentVertice] = new Vector3(currentBlock*20, vec1.y, 0);
			uv[currentVertice] = new Vector2(0, 1);
			currentVertice++;

			amt = ((float)currentBlock+1)/blocksToCreate;
			vec2 =	CalculateBezierPoint(amt, new Vector3(0,0,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(10,100,0), 
			                            new Vector3(blocksToCreate-1,0,0)
			                            );

			// Top Right of block
			vertices[currentVertice] = new Vector3((currentBlock*20)+20, vec2.y, 0);
			uv[currentVertice] = new Vector2(1, 1);
			currentVertice++;
			
			// Bottom Right of block
			vertices[currentVertice] = new Vector3((currentBlock*20)+20, vec2.y-50, 0);
			uv[currentVertice] = new Vector2(1, 0);
			currentVertice++;
			
			// Bottom Left of block
			vertices[currentVertice] = new Vector3(currentBlock*20, vec1.y-50, 0);
			uv[currentVertice] = new Vector2(0, 0);
			currentVertice++;

			triangles[currentTriangle++] = (currentBlock*4)+0;
			triangles[currentTriangle++] = (currentBlock*4)+2;
			triangles[currentTriangle++] = (currentBlock*4)+3;
			triangles[currentTriangle++] = (currentBlock*4)+0;
			triangles[currentTriangle++] = (currentBlock*4)+1;
			triangles[currentTriangle++] = (currentBlock*4)+2;
		}
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		renderer.sharedMaterial.mainTexture = texture;

		return mesh;
	}
 * 
 * 
	Mesh CreatePlaneMesh2()
	{
		//Material newMaterial = new Material(Shader.Find("Unlit/Transparent"));

		Mesh mesh = new Mesh();

			//new Vector3(-.5f,2f,0),
			//new Vector3(.5f,2f,0),
			//new Vector3(.5f,-2f,0),
			//new Vector3(-.5f,-2f,0)

		/// we need to create 100 points...

		int points = 100;
		int trianglePoints = 0;
		int topPoint = 0;

		Vector3[] vertices = new Vector3[points*2];
		Vector2[] uv = new Vector2[points * 2];
		int[] triangles = new int[50*6];

		for(int ix=0; ix<points; ) {
			// Top left of box
			vertices[ix] = new Vector3(topPoint, 1f, 0);
			uv[ix] = new Vector2(0, 1);
			ix++;

			// Top Right of box
			vertices[ix] = new Vector3(topPoint+2, 1f, 0);
			uv[ix] = new Vector2(1, 1);
			ix++;

			// Bottom Right of box
			vertices[ix] = new Vector3(topPoint+2, 0, 0);
			uv[ix] = new Vector2(1, 0);
			ix++;

			// Bottom Left of box
			vertices[ix] = new Vector3(topPoint, 0, 0);
			uv[ix] = new Vector2(0, 0);
			ix++;

			triangles[trianglePoints++] = topPoint+0;
			triangles[trianglePoints++] = topPoint+2;
			triangles[trianglePoints++] = topPoint+3;
			triangles[trianglePoints++] = topPoint+0;
			triangles[trianglePoints++] = topPoint+1;
			triangles[trianglePoints++] = topPoint+2;

			topPoint++;
		}

		//0,2,3,0,1,2
		//int[] triangles = new int[]
		//{
		//	0,2,3,0,1,2
		//};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		//newMaterial.mainTexture = texture;

		renderer.sharedMaterial.mainTexture = texture;

		return mesh;
	}
 * 
Mesh CreatePlaneMesh2()
	{
		//Material newMaterial = new Material(Shader.Find("Unlit/Transparent"));

		Mesh mesh = new Mesh();

			//new Vector3(-.5f,2f,0),
			//new Vector3(.5f,2f,0),
			//new Vector3(.5f,-2f,0),
			//new Vector3(-.5f,-2f,0)

		/// we need to create 100 points...

		Vector3[] vertices = new Vector3[]
		{
			new Vector3(0,2f,0),
			new Vector3(1,2.5f,0),
			new Vector3(1,-2f,0),
			new Vector3(0,-2f,0),



			new Vector3(1,2f,0),
			new Vector3(2,2.5f,0),
			new Vector3(2,-5f,0),
			new Vector3(1,-5f,0),


		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 0),

			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 0),

		};

		//0,2,3,0,1,2
		int[] triangles = new int[]
		{
			//0,2,3,0,1,2
			//,
			//4,6,7,4,5,6
			0,6,3
			,
			0,5,6
		};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		//newMaterial.mainTexture = texture;

		renderer.sharedMaterial.mainTexture = texture;

		return mesh;
	}
 */
