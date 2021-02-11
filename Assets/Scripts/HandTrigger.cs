using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using SimpleJSON;

public class HandTrigger : MonoBehaviour
{
    [Range(0, 10)]
    public int mSensitivity = 5; // dit is het getal dat de drempel bepaalt van het triggerpoints die zich in het vlak moeten begeven alvorens te 'triggeren'

    public bool mIsTriggered = false;

    private Camera mCamera = null;

    private RectTransform mRectTransform = null;
    private Image mImage = null;

    private AudioSource audioSource;

    private bool firstTrigger = false;
    private GameObject[] interactiepunten = null;

    private static readonly string url = "https://firestore.googleapis.com/v1/projects/geraaktfirebase/databases/(default)/documents/interactie/lastInteractie?updateMask.fieldPaths=timestamp";

    // video 
    GameObject video1;
    GameObject video2;
    GameObject video3;
    GameObject staticVideo;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        MeasureDepth.OnTriggerPoints += OnTriggerPoints;

        mCamera = Camera.main;

        mRectTransform = GetComponent<RectTransform>(); // component ophalen in dit geval is het component 'RectTransform'
        audioSource = mRectTransform.GetComponent<AudioSource>();
        mImage = GetComponent<Image>();

        // alle interactiepunten opvragen om deze dan te doen 'verdwijnen'
        interactiepunten = GameObject.FindGameObjectsWithTag("interactiepunt");

        video1 = GameObject.FindWithTag("VideoTrigger1"); 
        video2 = GameObject.FindWithTag("VideoTrigger2");
        video3 = GameObject.FindWithTag("VideoTrigger3");

        staticVideo = GameObject.FindWithTag("static");


        if (gameObject.name == "HandTrigger1")
        {
            videoPlayer = video1.GetComponent<VideoPlayer>();
            Debug.Log(videoPlayer);
        }else if(gameObject.name == "HandTrigger2")
        {
            videoPlayer = video2.GetComponent<VideoPlayer>();
            Debug.Log(videoPlayer); 
        }else if(gameObject.name == "HandTrigger3")
        {
            videoPlayer = video3.GetComponent<VideoPlayer>();
            Debug.Log(videoPlayer);
        }

        // video 
        
    }

    private void OnDestroy()
    {
        MeasureDepth.OnTriggerPoints -= OnTriggerPoints;
    }

    private void handleTrigger()
    {
        staticVideo.SetActive(false);
        audioSource.Play();
        videoPlayer.Play();

        StartCoroutine(PostInteractie());


        for (int i = 0; i < interactiepunten.Length; i++)
        {
            if (interactiepunten[i] != gameObject)
            {
                interactiepunten[i].SetActive(false);
                Debug.Log(interactiepunten[i]);
            }
        }

    }

    private void OnTriggerPoints(List<Vector2> triggerPoints)
    {
        if (!enabled)
            return;

        int count = 0;

        foreach(Vector2 point in triggerPoints)
        {

            Vector2 flippedY = new Vector2(point.x, mCamera.pixelHeight - point.y);

            if (RectTransformUtility.RectangleContainsScreenPoint(mRectTransform, flippedY))
                count++;
        }

        //checken dat het aantal points in screen groter is dan de sensitivity
        if(count > mSensitivity)
        {

            mIsTriggered = true;

            if(firstTrigger == false){
                firstTrigger = true;
                handleTrigger();
            }else{
                return; 
            }
        }

        if (count < mSensitivity)
        {

            mIsTriggered = false;
            firstTrigger = false;
            
            audioSource.Stop();
            videoPlayer.Stop();
 

            for (int i = 0; i < interactiepunten.Length; i++)
            {
                if (interactiepunten[i] != gameObject)
                {
                   interactiepunten[i].SetActive(true);
                }
              
            }
        }
    }

    #region Firebase REST API
    IEnumerator PostInteractie()
    {
        {

            string time = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

            JSONObject timestampValue = new JSONObject();
            timestampValue.Add("timestampValue", time);

            JSONObject timestamp = new JSONObject();
            timestamp.Add("timestamp", timestampValue);

            JSONObject field = new JSONObject();
            field.Add("fields", timestamp);

            string putData = field.ToString();

            var formData = System.Text.Encoding.UTF8.GetBytes(putData);

            UnityWebRequest postTimestamp = UnityWebRequest.Put(url, formData);

            postTimestamp.method = "PATCH";
            postTimestamp.SetRequestHeader("Content-Type", "application/json");

            yield return postTimestamp.SendWebRequest();

            if (postTimestamp.isNetworkError || postTimestamp.isNetworkError)
            {
                Debug.LogError("error" + postTimestamp.error);
            }
            else
            {
                Debug.Log("succes" + postTimestamp.downloadHandler.text);
            }
        }
    }
    #endregion


}
