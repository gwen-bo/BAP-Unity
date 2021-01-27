using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class imageViewer : MonoBehaviour
{

    public MeasureDepth mMeasureDepth;
    public MultiSourceManager mMultiSource;

    public RawImage mRawImage;
    public RawImage mRawDepth;

    // Update is called once per frame
    void Update()
    {
        mRawImage.texture = mMultiSource.GetColorTexture();

        mRawDepth.texture = mMeasureDepth.mDepthTexture;
    }
}
