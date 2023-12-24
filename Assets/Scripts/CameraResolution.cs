using UnityEngine;
using System.Collections.Generic;

public class CameraResolution : MonoBehaviour {
    private int ScreenX = 0;
    private int ScreenY = 0;


    private void Resize() {

        if (Screen.width == ScreenX && Screen.height == ScreenY) {
            return;
        }

        float tarasp = 21.0f / 18.0f;
        float winasp = (float)Screen.width / (float)Screen.height;
        float scaleh = winasp / tarasp;
        Camera camera = GetComponent<Camera>();

        if (scaleh < 1.0f) {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleh;
            rect.x = 0;
            rect.y = (1.0f - scaleh) / 2.0f;

            camera.rect = rect;
        }
        else {
            float scalew = 1.0f / scaleh;

            Rect rect = camera.rect;

            rect.width = scalew;
            rect.height = 1.0f;
            rect.x = (1.0f - scalew) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        ScreenX = Screen.width;
        ScreenY = Screen.height;
    }


    void OnPreCull() {
        if (Application.isEditor) return;
        Rect w = Camera.main.rect;
        Rect n = new Rect(0, 0, 1, 1);

        Camera.main.rect = n;
        GL.Clear(true, true, Color.black);

        Camera.main.rect = w;

    }

    void Start() {
        Resize();
    }

    void Update() {
        Resize();
    }
}