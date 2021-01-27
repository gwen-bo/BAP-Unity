using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectTrigger : MonoBehaviour
{
    [Range(0, 10)]
    public int mSensitivity = 5; // dit is het getal dat de drempel bepaalt van het triggerpoints die zich in het vlak moeten begeven alvorens te 'triggeren'

    public bool mIsTriggered = false;

    private Camera mCamera = null;

    private RectTransform mRectTransform = null;
    private Image mImage = null;

    private AudioSource audioSource;

    private bool firstTrigger = false;
    private bool playAudio = false; 


    private void Awake()
    {
        MeasureDepth.OnTriggerPoints += OnTriggerPoints;

        mCamera = Camera.main;

        mRectTransform = GetComponent<RectTransform>(); // component ophalen in dit geval is het component 'RectTransform'
        audioSource = mRectTransform.GetComponent<AudioSource>();
        mImage = GetComponent<Image>();

    }

    private void OnDestroy()
    {
        MeasureDepth.OnTriggerPoints -= OnTriggerPoints;
    }

    private void handleAudio()
    {
        audioSource.Play();
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
                handleAudio();
            }else{
                return; 
            }

            // zegt wat e moet gebeuren als het getriggerd is
            mImage.color = Color.red; // getriggerd -> kleur rood
        }

        if (count < mSensitivity)
        {
            mIsTriggered = false;
            firstTrigger = false;
            // zegt wat e moet gebeuren als het getriggerd is
            mImage.color = Color.black; // getriggerd -> kleur rood

            audioSource.Stop();
        }
    }

}
