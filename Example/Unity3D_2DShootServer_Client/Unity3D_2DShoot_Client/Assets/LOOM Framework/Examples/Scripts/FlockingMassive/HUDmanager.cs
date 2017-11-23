using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDmanager : MonoBehaviour 
{
    private Vector2 guiScale = new Vector2(1f, 1f);
    public float IntroDuration = 3f;
    public Texture2D IntroScreen;
    private float fadeAlpha = 1f;



    public int panelIdx = 0;
    private float particleDestinationLerp = 0f;
    

    private MassiveFlockingExample flockingManager;
    private Transform[] Cubes;

	// Use this for initialization
	void Start () 
    {
        flockingManager = (MassiveFlockingExample)GameObject.FindObjectOfType(typeof(MassiveFlockingExample));

        List<Transform> cubeList = new List<Transform>();
        GameObject[] colliders = GameObject.FindGameObjectsWithTag("EnvironmentCollider");
        foreach (GameObject coll in colliders)
        {
            if (coll.name == "Cube")
            {
                cubeList.Add(coll.transform);
            }
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            guiScale = new Vector2(1.5f, 1.5f);

        Cubes = cubeList.ToArray();
        UpdateStickToFrame();
	}



    private void UpdateStickToFrame()
    {
        flockingManager.FlockingDestinationAttractRadius = Mathf.Lerp(0, flockingManager.FlockingBoundsRadius, particleDestinationLerp);
        flockingManager.FlockingDestinationReachedRadius = Mathf.Lerp(0, 20, particleDestinationLerp);
        flockingManager.ParticleSize = Mathf.Lerp(1.5f, 2.5f, particleDestinationLerp);

        float cubeScale = Mathf.Lerp(7, 1, particleDestinationLerp);
        foreach (Transform cube in Cubes)
            cube.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
    }















    void OnGUI()
    {

        GUI.color = panelIdx == 0 ? Color.grey : panelIdx == 1 ? Color.cyan : Color.yellow ;
        if (GUI.Button(new Rect(0, 0, Screen.width, 40), panelIdx == 0 ? "Tap here to toggle modes" : panelIdx == 1 ? "Flocking settings" : "Threading settings"))
        {
            panelIdx ++;
            if (panelIdx >= 3)
                panelIdx = 0;
        }

        GUI.color = Color.white;
        Matrix4x4 mBackup = GUI.matrix;


        //Flocking settings
        if (panelIdx == 1) 
        {
            Rect PanelRect = new Rect(0, 50, Screen.width / guiScale.x, 200);
            GUIUtility.ScaleAroundPivot(guiScale, Vector2.zero);
            GUI.Box(PanelRect, string.Empty);
            GUILayout.BeginArea(PanelRect);


            
            //--------------- Stick-to-Frame Slider --------------------
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Randomize", GUILayout.Width(85), GUILayout.Height(85)))
            {
                //flockingManager.FlockingSteeringSpeed = Random.Range(0f, 4f);
                //flockingManager.FlockingBoundsRadius = Random.Range(25f, 150f);

                flockingManager.FlockingSeperationWeight = Random.Range(0, 10f);
                flockingManager.FlockingSeperationRadius = Random.Range(0f, flockingManager.FlockingBoundsRadius);

                flockingManager.FlockingCohesionWeight = Random.Range(0, 10f);
                flockingManager.FlockingCohesionRadius = Random.Range(0f, flockingManager.FlockingBoundsRadius);

                flockingManager.FlockingAlignmentWeight = Random.Range(0, 10f);
                flockingManager.FlockingAlignmentRadius = Random.Range(0f, flockingManager.FlockingBoundsRadius);
            }

            GUILayout.BeginVertical();
            DrawSlider("Stick to Frame", ref particleDestinationLerp, 0f, 1f);
            DrawSlider("BoundsRadius", ref flockingManager.FlockingBoundsRadius, 25f, 150f);
            DrawSlider("Steer Speed", ref flockingManager.FlockingSteeringSpeed, 0f, 4f);
            GUILayout.EndVertical();

            UpdateStickToFrame();
            GUILayout.EndHorizontal();
            //--------------- Stick-to-Frame Slider --------------------


            GUILayout.BeginVertical();
            GUILayout.Space(15);
            
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            DrawSlider("Seperation W", ref flockingManager.FlockingSeperationWeight, 0f, 10);
            DrawSlider("Alignment W", ref flockingManager.FlockingAlignmentWeight, 0f, 10);
            DrawSlider("Cohesion W", ref flockingManager.FlockingCohesionWeight, 0f, 10);
            GUILayout.EndVertical();


            GUILayout.BeginVertical();
            DrawSlider("Seperation R", ref flockingManager.FlockingSeperationRadius, 0f, flockingManager.FlockingBoundsRadius);
            DrawSlider("Alignment R", ref flockingManager.FlockingAlignmentRadius, 0f, flockingManager.FlockingBoundsRadius);
            DrawSlider("Cohesion R", ref flockingManager.FlockingCohesionRadius, 0f, flockingManager.FlockingBoundsRadius);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
            
            GUI.matrix = mBackup;
        }

        //Threading Settings
        else if (panelIdx == 2)
        {
            Rect PanelRect = new Rect(0, 50, Screen.width / guiScale.x, 100);
            GUIUtility.ScaleAroundPivot(guiScale, Vector2.zero);
            GUI.Box(PanelRect, string.Empty);
            GUILayout.BeginArea(PanelRect);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUI.color = flockingManager.MultithreadingEnabled ? Color.green : Color.red;
            if (GUILayout.Button(flockingManager.MultithreadingEnabled ? "Multi Threading Enabled" : " Multi Threading Disabled", GUILayout.Height(50)))
                flockingManager.MultithreadingEnabled = !flockingManager.MultithreadingEnabled;

            GUI.color = Color.white;
            if (GUILayout.Button("Add 500 boids", GUILayout.Height(50)))
                flockingManager.FlockingSpawnCount += 500;
            
            if (GUILayout.Button("Remove 500 boids", GUILayout.Height(50)))
                flockingManager.FlockingSpawnCount = Mathf.Max(0, flockingManager.FlockingSpawnCount - 500);
            
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUI.color = Color.white;
            GUILayout.Space(20);
            GUI.enabled = flockingManager.MultithreadingEnabled;
            
            GUILayout.BeginVertical();
            DrawTextField("Number of Threads",  ref flockingManager.ThreadingMaxThreads);
            GUILayout.EndVertical();
            
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUI.matrix = mBackup;
        }


        //--------------- FPS Feedback --------------------
        if (flockingManager.myThreadScheduler != null)
        {
            Rect totalBar = new Rect(20, Screen.height - 125, 350, 25);
            DrawProgressBar(totalBar, "FLocking Behaviour Progress: ", flockingManager.myThreadScheduler.Progress);
        }

        Rect particleCountRect = new Rect(20, Screen.height - 100, 350, 25);
        GUI.Label(particleCountRect, "Number of Boids: " + flockingManager.FlockingSpawnCount);

        Rect flockingFpsRect = new Rect(20, Screen.height - 75, 350, 20);
        GUI.Label(flockingFpsRect, "FLocking Behaviour FPS: " + (1f / flockingManager.flockingUpdateTime));

        Rect gameFpsRect = new Rect(20, Screen.height - 50, 350, 20);
        float gameFPS = (1f / Time.deltaTime);

        GUI.color = gameFPS < 20f ? Color.red : Color.white;
        GUI.Label(gameFpsRect, "Game FPS: " + gameFPS);

        GUI.color = Color.white;
        //--------------- FPS Feedback --------------------


        //--------------- Intro Screen --------------------
        if (Time.realtimeSinceStartup < IntroDuration)
        {
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), IntroScreen, ScaleMode.ScaleAndCrop);
        }
        else
        {
            fadeAlpha -= Time.deltaTime;
            if (fadeAlpha > 0f)
            {
                GUI.color = new Color(1f, 1f, 1f, fadeAlpha);
                GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), IntroScreen, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.enabled = true;
            }
        } 
        //--------------- Intro Screen --------------------
			
    }







    //--------------- GUI Calls --------------------
    private void DrawTextField(string prefix, ref int value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(prefix + ": ", GUILayout.Width(150));

        string strValue = value.ToString();
        strValue = GUILayout.TextField(strValue);
        int.TryParse(strValue, out value);

        GUILayout.EndHorizontal();
    }

    private void DrawSlider(string prefix, ref float value, float minValue, float maxValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(prefix + ": " + RoundWithPrecision(value, 1).ToString(), GUILayout.Width(125));

        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(25)))
            value = Mathf.Clamp(value - ((maxValue - minValue) / 5f), minValue, maxValue);

        value = GUILayout.HorizontalSlider(value, minValue, maxValue);

        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(25)))
            value = Mathf.Clamp(value + ((maxValue - minValue) / 5f), minValue, maxValue);

        GUILayout.EndHorizontal();
    }

    public static float RoundWithPrecision(float value, int precision)
    {
        float multiplier = Mathf.Pow(10, (float)precision);
        value *= multiplier;
        value = Mathf.Round(value);

        return value / multiplier;
    }


    private void DrawProgressBar(Rect rect, string prefix, float progress)
    {
        GUI.Box(rect, prefix + Mathf.Round(progress * 100f).ToString());
        rect.width *= progress;

        GUI.color = Color.green;
        GUI.Box(rect, string.Empty);
        GUI.color = Color.white;
    }

    //--------------- GUI Calls --------------------
			
}
