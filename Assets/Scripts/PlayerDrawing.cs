using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrawing : MonoBehaviour {

    public Color color;

    HatBoard hatBoard;

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        hatBoard = GetComponent<HatBoard>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Mouse0)) {
            Vector2Int pos = mouseBoardPosition();
            if (pos.x != -1) {
                hatBoard.SetPixel(pos.x, pos.y, color);
            }
        }
        if (Input.GetKey(KeyCode.Mouse1)) {
            Vector2Int pos = mouseBoardPosition();
            if (pos.x != -1) {
                hatBoard.SetPixel(pos.x, pos.y, new Color(0, 0, 0, 0));
            }
        }
    }

    public void SetColor(Color newColor) {
        color = newColor;
    }

    Vector2Int mouseBoardPosition() {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
        Vector2 pixelCoord = Vector2.zero;
        if (newRect.Contains(Input.mousePosition)) {
            Vector2 size = new Vector2(16, 16);
            pixelCoord = Input.mousePosition - corners[0];
            pixelCoord /= rectTransform.rect.size;
            pixelCoord *= size;
        } else {
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int((int)pixelCoord.x / 32, (int)pixelCoord.y / 32);
    }
}
