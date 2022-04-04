using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraEffect : MonoBehaviour
{

    public float intensity;
    public Material material;

    public GameObject character;

    void Update()
    {
        GameObject[] lPlayerHeadGOs = GameObject.FindGameObjectsWithTag("Player");

            if (lPlayerHeadGOs.Length > 0)
            {
            character = lPlayerHeadGOs[0];
            }
    }
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /*if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }*/

        if (character != null) {
            intensity = Mathf.Min(0.0f, 30.0f * (character.transform.position.y - gameObject.transform.position.y));
        }

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);
    }
}
