using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LayeredImageLoader : MonoBehaviour
{
	public int imageStart = 1;
	public int imageStop = 47;
	public int iSkip = 2;
	public string path1 = "Images/Pole_cells_red/";
	public string extension = "jpg";

	Texture2D[] images;
	Texture2D [] sideImages;
	Texture2D [] frontImages;
	Color[,,] allPixels;
	public GameObject layeredImagePRE;
	GameObject[] layeredImagesObjects;
	GameObject[] frontImagesObjects;
	GameObject[] sideImagesObjects;
	int nImages;
	public Slider alphaSlider;
	public Slider cutoffSlider;
	int width = 0;
	int height = 0;

	GameObject topView;
	GameObject frontView;
	GameObject sideView;

	public float alphaInit = 0.3f;
	public float cutoffInit = 1.0f;
	public float normalInit = 1.0f;

	public bool extraPlanes = false;

	bool showSide;
	bool showFront;

	bool leaveMarker = false;


	void Start()
	{

		showSide = extraPlanes;
		showFront = extraPlanes;
		topView = new GameObject();
		topView.transform.parent = transform;
		topView.transform.localPosition = Vector3.zero;
		topView.transform.localScale = Vector3.one;
		topView.transform.localRotation = Quaternion.Euler(0, 0, 0);

		bool firstPass = true;
		nImages=0;
		if (imageStop > imageStart)
		{
			for(int i=imageStart;i<=imageStop;i+= iSkip) nImages++;
		}
		else
		{
			for (int i = imageStart; i >= imageStop; i -= iSkip) nImages++;
		}
		images = new Texture2D[nImages];
		layeredImagesObjects = new GameObject[nImages];
		
		for (int k = 0; k < nImages; k++)
		{
			int imageNumber=0;
			if(imageStop>imageStart)
			{
				imageNumber = iSkip*k+imageStart;
			} else
			{
				imageNumber = imageStart-iSkip*k;
			}
			string loadPath = path1 + string.Format("{0:D5}", imageNumber);
			Debug.Log(loadPath);
			images[k] = Resources.Load<Texture2D>(loadPath) as Texture2D;

			if (firstPass)
			{
				firstPass = false;
				width = images[k].width;
				height = images[k].height;
				allPixels = new Color[width,height,nImages];
			}
			Color [] pixels = images[k].GetPixels();
			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					if (i > width / 2 && j > height / 2 && k > nImages / 2 && leaveMarker)
					{
						pixels[j * width + i].b = 1.0f;
					}
					allPixels[i,j,k] = pixels[j*width+i];

				}
			}
			images[k].SetPixels(pixels);
			images[k].Apply();
			layeredImagesObjects[k] = Instantiate(layeredImagePRE);
			layeredImagesObjects[k].transform.parent = topView.transform;
			layeredImagesObjects[k].transform.localPosition = new Vector3(0, (float)k / (float)(nImages - 1) - 0.5f, 0);
			layeredImagesObjects[k].transform.localScale = Vector3.one;
			layeredImagesObjects[k].transform.localRotation = Quaternion.Euler(90, 0, 0);

			layeredImagesObjects[k].GetComponent<Renderer>().material.mainTexture = images[k];
		}

		if(showFront) { 
			// front view
			frontImages = new Texture2D[height];
			frontImagesObjects = new GameObject[height];
			frontView = new GameObject();
			frontView.transform.parent = transform;
			frontView.transform.localPosition = Vector3.zero;
			frontView.transform.localScale = Vector3.one;
			frontView.transform.localRotation = Quaternion.Euler(0, 0, 0);

			for (int j=0;j<height;j++)
			{
				frontImages[j] = new Texture2D(width,nImages);
				Color []pixels = new Color[width*nImages];
				for(int k=0;k<nImages;k++)
				{
					for(int i=0;i<width;i++)
					{
						pixels[k*width+i] = allPixels[i, j, k];
					}
				}
				frontImages[j].SetPixels(pixels);
				frontImages[j].Apply();
				frontImagesObjects[j] = Instantiate(layeredImagePRE);
				frontImagesObjects[j].transform.parent = frontView.transform;
				frontImagesObjects[j].transform.localPosition = new Vector3(0, 0, (float)j / (float)(height - 1)-0.5f);
				frontImagesObjects[j].transform.localRotation = Quaternion.Euler(0, 0, 0);
				frontImagesObjects[j].transform.localScale = Vector3.one;
				frontImagesObjects[j].GetComponent<Renderer>().material.mainTexture = frontImages[j];
			}
		}

		if(showSide) { 
			// side view
			sideImages = new Texture2D[width];
			sideImagesObjects = new GameObject[width];
			sideView = new GameObject();
			sideView.transform.parent = transform;
			sideView.transform.localPosition = Vector3.zero;
			sideView.transform.localScale = Vector3.one;
			sideView.transform.localRotation = Quaternion.Euler(0, 0, 0);

			for (int i = 0; i < width; i++)
			{
				sideImages[i] = new Texture2D(height, nImages);
				Color[] pixels = new Color[height * nImages];
				for (int k = 0; k < nImages; k++)
				{
					for (int j = 0; j < height; j++)
					{
						pixels[k * height + j] = allPixels[i, j, k];
					}
				}
				sideImages[i].SetPixels(pixels);
				sideImages[i].Apply();
				sideImagesObjects[i] = Instantiate(layeredImagePRE);
				sideImagesObjects[i].transform.parent = sideView.transform;
				sideImagesObjects[i].transform.localPosition = new Vector3((float)i / (float)(width - 1) - 0.5f,0,0);
				sideImagesObjects[i].transform.localScale = Vector3.one;
				sideImagesObjects[i].transform.localRotation = Quaternion.Euler(180,90,180);
				//sideImagesObjects[i].transform.Rotate(sideImagesObjects[i].transform.right, 90);
				//sideImagesObjects[i].transform.Rotate(sideImagesObjects[i].transform.up, -90);
				sideImagesObjects[i].GetComponent<Renderer>().material.mainTexture = sideImages[i];
			}
		}

		alphaSlider.value = alphaInit;
		cutoffSlider.value = cutoffInit;
		setAlpha(alphaInit);
		setCutoff(cutoffInit);
		setNormal(normalInit);

	}

	public void setAllRenderers(string property, float value)
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in renderers)
		{
			rend.material.SetFloat(property, value);
		}
	}

	public void setNormal(float normal)
	{
		setAllRenderers("_NormalCutoff", normal);

	}

	public void setAlpha(float alpha)
	{

		setAllRenderers("_Alpha", alpha);
	}

	public void setCutoff(float cutoff)
	{
		setAllRenderers("_Cutoff", cutoff);


		
	}

	// Update is called once per frame
	void Update()
	{

	}
}
