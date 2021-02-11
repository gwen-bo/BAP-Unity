using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoHandlerScript : MonoBehaviour
{
    VideoPlayer video1;
    VideoPlayer video2;
    VideoPlayer video3;
    VideoPlayer staticVideo;
    GameObject staticImage; 

    // Start is called before the first frame update
    void Start()
    {

        video1 = GameObject.FindWithTag("VideoTrigger1").GetComponent<VideoPlayer>();
        video2 = GameObject.FindWithTag("VideoTrigger2").GetComponent<VideoPlayer>();
        video3 = GameObject.FindWithTag("VideoTrigger3").GetComponent<VideoPlayer>();
        staticVideo = GameObject.FindGameObjectWithTag("VideoStatic").GetComponent<VideoPlayer>();
        staticImage = GameObject.FindGameObjectWithTag("static");
    }

    // Update is called once per frame
    void Update()
    {
        if (video1.isPlaying || video2.isPlaying || video3.isPlaying)
        {
            staticVideo.Stop();
            staticImage.SetActive(false);
        }else
        {
            staticImage.SetActive(true);
            staticVideo.Play();
        }

        
    }
}