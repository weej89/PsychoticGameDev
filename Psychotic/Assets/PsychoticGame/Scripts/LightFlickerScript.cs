using UnityEngine;
using System.Collections;

public enum enColorchannels
{
	all = 0,
	red = 1,
	blue = 2,
	green = 3
}

public enum enWaveFunctions
{
	sinus = 0,
	triangle = 1,
	square = 2,
	sawtooth = 3,
	inverted_saw = 4,
	noise = 5
}

public class LightFlickerScript : MonoBehaviour {

	/*
	private Light lightSource;
	private double currentFlickerTime;
	private double currentTime;
	public double minFlickerTime;
	public double maxFlickerTime;

	// Use this for initialization
	void Start () 
	{
		lightSource = GetComponent<Light> ();
		currentFlickerTime = GetCurrentFlickerTime ();
		currentTime = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (CountdownTimer()) 
		{
			lightSource.enabled = !(lightSource.enabled); 
			GetCurrentFlickerTime();
		}
	}

	private float GetCurrentFlickerTime()
	{
		return Random.Range((float)minFlickerTime, (float)maxFlickerTime);
	}

	private bool CountdownTimer()
	{
		bool timerComplete = false;

		if (currentTime < currentFlickerTime) 
		{
			currentTime += Time.deltaTime;
		}
		else 
		{
			currentTime=0;
			timerComplete=true;
		}

		return timerComplete;
	}
	*/

	public enColorchannels colorChannel = enColorchannels.all;
	public enWaveFunctions waveFunction = enWaveFunctions.sinus;
	public float offset = 0.0f; //constant offet
	public float amplitude = 1.0f; //amplitude of the wave
	public float phase = 0.0f; //start point inside on wave cycle
	public float frequency = 0.5f; //cycle frequency per second
	public bool affectsIntesity = true;

	//Keep a copy of the original values
	private Color originalColor;
	private float originalIntesity;

	void Start()
	{
		originalColor = GetComponent<Light> ().color;
		originalIntesity = GetComponent<Light> ().intensity;
	}

	void Update()
	{
		Light light = GetComponent<Light> ();

		if(affectsIntesity)
			light.intensity=originalIntesity * EvalWave();

		Color o = originalColor;
		Color c = GetComponent<Light>().color;

		if(colorChannel == enColorchannels.all)
			light.color=originalColor * EvalWave();
		else if(colorChannel == enColorchannels.red)
			light.color=new Color(o.r*EvalWave(), c.g, c.b, c.a);
		else if(colorChannel == enColorchannels.green)
			light.color=new Color(c.r, o.g * EvalWave(), c.b, c.a);
		else //blue
			light.color = new Color(c.r, c.g, o.b *EvalWave(), c.a);
	}

	private float EvalWave()
	{
		float x = (Time.time + phase) * frequency;
		float y;

		x = x - Mathf.Floor (x); //normalized value (0..1)

		if (waveFunction == enWaveFunctions.sinus)
		{
			y = Mathf.Sin (x * 2f * Mathf.PI);
		}
		else if (waveFunction == enWaveFunctions.triangle) 
		{
			if (x < .5f)
				y = 4.0f * x - 1.0f;
			else
				y = -4.0f * x + 3.0f;
		}
		else if (waveFunction == enWaveFunctions.square) 
		{
			if (x < .5f)
				y = 1.0f;
			else
				y = -1.0f;
		}
		else if (waveFunction == enWaveFunctions.triangle) 
		{
			y = x;
		}
		else if (waveFunction == enWaveFunctions.inverted_saw) 
		{
			y = 1.0f - x;
		}
		else if (waveFunction == enWaveFunctions.noise) 
		{
			y = 1f - (Random.value * 2f);
		}
		else
		{
			y = 1.0f;
		}
		return (y*amplitude) + offset;
	}

}
