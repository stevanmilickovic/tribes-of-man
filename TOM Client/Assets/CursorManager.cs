using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D cursorPressedTexture;

    private Vector2 cursorHotspot;

    float previousScroll = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width, cursorTexture.height);
        //Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    // Update is called once per frame 
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            Cursor.SetCursor(cursorPressedTexture, cursorHotspot, CursorMode.Auto);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || (Input.GetAxisRaw("Mouse ScrollWheel") == 0 && previousScroll != 0f) || (Input.GetAxisRaw("Mouse ScrollWheel") == 0 && previousScroll != 0f))
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }

        previousScroll = Input.GetAxisRaw("Mouse ScrollWheel");*/
    }
}
