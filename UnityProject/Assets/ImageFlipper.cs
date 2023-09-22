using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFlipper : MonoBehaviour
{
    public Sprite [] images;
    float time = 0.0f;
    float delay = 0.2f;
    int imageNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time>delay)
		{
            time = 0.0f;
            imageNo += 1;
            imageNo %= images.Length;
            GetComponent<Image>().sprite = images[imageNo];
		}
    }
}
