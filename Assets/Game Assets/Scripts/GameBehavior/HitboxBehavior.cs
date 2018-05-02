using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxBehavior : MonoBehaviour
{
    private Renderer render;
    public GameLogic gameLogic;
    public Texture2D cursorGrabTexture;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    void OnMouseEnter()
    {
        render.enabled = true;
        Cursor.SetCursor(cursorGrabTexture, Vector2.zero, CursorMode.Auto);
    }
    void OnMouseExit()
    {
        render.enabled = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseUpAsButton()
    {
        int column = int.Parse(this.tag.Replace("column_", ""));
        gameLogic.RegisterPlayerMovement(column);
    }
}
