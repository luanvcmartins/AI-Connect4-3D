using UnityEngine;
using System.Collections;

public class HitBoxMaterialBehavior : MonoBehaviour
{
    private Renderer render;
    public float scrollSpeed = 0.9f;
    public float scrollSpeed2 = 0.9f;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        float offset = Time.time * scrollSpeed;
        float offset2 = Time.time * scrollSpeed2;

        render.material.mainTextureOffset = new Vector2(offset2, -offset);
    }
}