using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Frankfort.Threading;


public struct Pixel
{
    public int x;
    public int y;
    
    public Pixel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}


public class TextureBlurThreaded : MonoBehaviour
{
    public bool MultithreadingEnabled = false;
    public int MaxThreads = -1;
    public bool safeMode = true;

    public Texture2D Texture;
    public int BlurRange = 1;
    public int BlurSamples = 5;
    public bool NinePointsBlur = false;

    private ThreadPoolScheduler myScheduler;
    private Color32[] origionalColors;
    private Color32[] sourceColors;
    private Color32[] destinationColors;
    private Pixel[] sourcePixels;

    private int textureWidth;
    private int textureHeight;
    private Texture2D displayTexture;

    float totaProgress = 0f;
    float timePerBlur = 1f;


    private Vector2 guiScale = new Vector2(1f, 1f);



	// Use this for initialization
    void awake()
    {
        Application.targetFrameRate = 25;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            guiScale = new Vector2(1.5f, 1.5f);
    }



	void Start () 
    {
        myScheduler = Loom.CreateThreadPoolScheduler("myScheduler");

        displayTexture = new Texture2D(Texture.width, Texture.height, TextureFormat.ARGB32, false);
        textureWidth = Texture.width;
        textureHeight = Texture.height;

        origionalColors = Texture.GetPixels32();
        sourcePixels = new Pixel[origionalColors.Length];
        sourceColors = new Color32[origionalColors.Length];
        destinationColors = new Color32[origionalColors.Length];

        int i =0;
        for (int y = 0; y < Texture.height; y++)
        {
            for (int x = 0; x < Texture.width; x++)
            {
                sourcePixels[i] = new Pixel(x, y);
                i++;
            }
        }

        myScheduler.ForceToMainThread = !MultithreadingEnabled;
        StartBluringTexture();
	}




    private void StartBluringTexture()
    {
        StartCoroutine(BlurTextureSampled());
    }


    private IEnumerator BlurTextureSampled()
    {
        while (true)
        {
            totaProgress = 0f;
            yield return new WaitForEndOfFrame();

            myScheduler.ForceToMainThread = !MultithreadingEnabled;
            Array.Copy(origionalColors, sourceColors, origionalColors.Length);

            float time = Time.realtimeSinceStartup;

            if (BlurSamples > 0)
            {
                for (int i = 0; i < BlurSamples; i++)
                {
                    Loom.StartMultithreadedWorkloadExecution<Pixel>(BlurPixels, sourcePixels, onComplete, onPackageComplete, MaxThreads, myScheduler, safeMode);

                    while (myScheduler.isBusy)
                    {
                        totaProgress = (float)i / (float)BlurSamples;
                        totaProgress += myScheduler.Progress / (float)BlurSamples;
                        yield return new WaitForSeconds(0.001f);
                    }

                    //--------------- Blit --------------------
                    if (i < BlurSamples - 1)
                    {
                        Color32[] oldSource = sourceColors;
                        sourceColors = destinationColors;
                        destinationColors = oldSource;
                    }
                    //--------------- Blit --------------------
                }

                timePerBlur = Time.realtimeSinceStartup - time;
                displayTexture.SetPixels32(destinationColors);
                displayTexture.Apply();
            }
            else
            {
                timePerBlur = Time.realtimeSinceStartup - time;
                displayTexture.SetPixels32(sourceColors);
                displayTexture.Apply();
            }

        }
    }

    private void onComplete(Pixel[] workLoad)
    {
        //Debug.Log("All workload processed: " + workLoad.Length);
    }

    private void onPackageComplete(Pixel[] workLoadPartial, int firstIndex, int lastIndex)
    {
        //Debug.Log("partial workload processed: " + workLoadPartial.Length + ", from: " + firstIndex + ", to:  "+ lastIndex);
    }


    public void BlurPixels(Pixel pixel, int i)
    {
        int r = 0;
        int g = 0;
        int b = 0;
        int range = Math.Max(1, BlurRange);

        int xMin = Mathf.Max(0, pixel.x - range);
        int yMin = Mathf.Max(0, pixel.y - range);

        if (!NinePointsBlur)
        {
            int xMax = Mathf.Min(textureWidth, pixel.x + range);
            int yMax = Mathf.Min(textureHeight, pixel.y + range);

            int total = 0;
            for (int y = yMin; y < yMax; y++)
            {
                int yRow = y * textureWidth;
                for (int x = xMin; x < xMax; x++)
                {
                    Color32 color = sourceColors[x + yRow];
                    r += color.r;
                    g += color.g;
                    b += color.b;
                    total++;
                }
            }
            destinationColors[i] = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), (byte)255);
        }
        else
        {
            int xMax = Mathf.Min(textureWidth - 1, pixel.x + range);
            int yMax = Mathf.Min(textureHeight - 1, pixel.y + range);

            Color32
            color = getColorFromXY(xMin, yMin); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(xMax, yMin); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(xMax, yMax); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(xMin, yMax); r += color.r; g += color.g; b += color.b;

            color = getColorFromXY(pixel.x, pixel.y); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(xMin, pixel.y); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(xMax, pixel.y); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(pixel.x, yMin); r += color.r; g += color.g; b += color.b;
            color = getColorFromXY(pixel.x, yMax); r += color.r; g += color.g; b += color.b;

            destinationColors[i] = new Color32((byte)(r / 9), (byte)(g / 9), (byte)(b / 9), (byte)255);
        }
    }

    private Color32 getColorFromXY(int x, int y)
    {
        return sourceColors[x + (y * textureWidth)];
    }


    private void OnGUI()
    {
        if (displayTexture != null)
        {

            //--------------- FPS Feedback --------------------
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(screenRect, displayTexture, ScaleMode.ScaleAndCrop);

            Rect totalBar = new Rect(20, 20, 200, 25);
            DrawProgressBar(totalBar, "Total: ", totaProgress);

            Rect sampleBar = new Rect(20, 50, 200, 20);
            DrawProgressBar(sampleBar, "per sample: ", myScheduler.Progress);


            Rect blurFpsRect = new Rect(20, 90, 200, 20);
            GUI.Label(blurFpsRect, "Blur FPS: " + (1f / timePerBlur));

            Rect gameFpsRect = new Rect(20, 110, 200, 20);
            float gameFPS = (1f / Time.deltaTime);

            GUI.color = gameFPS < 20f ? Color.red : Color.white;
            GUI.Label(gameFpsRect, "Game FPS: " + gameFPS); 
            //--------------- FPS Feedback --------------------
			




            Rect PanelRect = new Rect(225, 20, (Screen.width / guiScale.x) - 250, 115 );
            GUIUtility.ScaleAroundPivot(guiScale, Vector2.zero);
            GUI.Box(PanelRect, string.Empty);
            GUILayout.BeginArea(PanelRect);


            GUILayout.BeginVertical();
            GUI.color = Color.white;

            //--------------- Blur Options --------------------
            DrawSlider("Blur Range", ref BlurRange, 1, 20);
            DrawSlider("Blur Samples", ref BlurSamples, 0, 10);
            //--------------- Blur Options --------------------
			


            //--------------- Threading Options --------------------
            GUILayout.BeginHorizontal();
            GUI.color = MultithreadingEnabled? Color.green : Color.red;
            if (GUILayout.Button(MultithreadingEnabled ? "Multi Threading Enabled" : "Multi Threading Disabled", GUILayout.Width(175), GUILayout.Height(50)))
                MultithreadingEnabled = !MultithreadingEnabled;

            GUI.color = Color.white;
            GUILayout.Space(20);
            GUI.enabled = !myScheduler.ForceToMainThread;

            GUILayout.BeginVertical();
            GUILayout.Space(20);
            DrawTextField("Number of Threads", ref MaxThreads);
            //DrawTextField("Packages per Thread", ref flockingManager.ThreadingPoolPackages);
            GUILayout.EndVertical();

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            //--------------- Threading Options --------------------
            
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private void DrawProgressBar(Rect rect, string prefix, float progress)
    {
        GUI.Box(rect, prefix + Mathf.Round(progress * 100f).ToString());
        rect.width *= progress;

        GUI.color = Color.green;
        GUI.Box(rect, string.Empty);
        GUI.color = Color.white;
    }


    private void DrawTextField(string prefix, ref int value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(prefix + ": ", GUILayout.Width(150));

        string strValue = value.ToString();
        strValue = GUILayout.TextField(strValue);
        int.TryParse(strValue, out value);

        GUILayout.EndHorizontal();
    }

    private void DrawSlider(string prefix, ref int value, int minValue, int maxValue, int increment = 1 )
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(prefix + ": " + value.ToString(), GUILayout.Width(125));

        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(25)))
            value = Mathf.Clamp(value - increment, minValue, maxValue);

        value = (int)GUILayout.HorizontalSlider((float)value, (float)minValue, (float)maxValue);

        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(25)))
            value = Mathf.Clamp(value + increment, minValue, maxValue);

        GUILayout.EndHorizontal();
    }

}
