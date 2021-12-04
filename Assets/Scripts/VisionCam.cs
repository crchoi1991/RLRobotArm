using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCam : MonoBehaviour
{
    RenderTexture vision;
    Texture2D tex;
    Rect rt;
    public Color32[] camImage;
    // Start is called before the first frame update
    void Start()
    {
        var cam = gameObject.GetComponent<Camera>();
        vision = cam.targetTexture;
        tex = new Texture2D(vision.width, vision.height);
        rt = new Rect(0, 0, vision.width, vision.height);
    }

    // Update is called once per frame
    void Update()
    {
        var prev = RenderTexture.active;
        RenderTexture.active = vision;
        tex.ReadPixels(rt, 0, 0);
        tex.Apply();
        RenderTexture.active = prev;

        camImage = tex.GetPixels32();
    }
}
