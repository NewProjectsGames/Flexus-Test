using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DecalPainter : MonoBehaviour
{
    public static DecalPainter Instance;
    public Texture2D decalTexture; // Текстура для декали


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Test

/*
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                PaintDecal(hit);
            }
        }
    }
*/

    #endregion

    public Vector2 decalSize = new Vector2(90, 90);

    public void PaintDecal(RaycastHit hit)
    {
        Renderer renderer = hit.transform.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        Material material = renderer.material;
        Texture2D mainTexture = material.mainTexture as Texture2D;
        if (mainTexture == null)
        {
            Color fillColor = Color.white;
            Texture2D texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false, true);
            for (int x1 = 0; x1 < texture.width; x1++)
            {
                for (int y1 = 0; y1 < texture.height; y1++)
                {
                    texture.SetPixel(x1, y1, fillColor);
                }
            }

            texture.Apply();
            material.mainTexture = texture;

            mainTexture = material.mainTexture as Texture2D;
        }

        if (mainTexture == null)
        {
            return;
        }


        Vector2 uv = hit.textureCoord;


        int decalWidth = (int)decalSize.x;
        int decalHeight = (int)decalSize.y;
        int textureWidth = mainTexture.width;
        int textureHeight = mainTexture.height;


        int x = Mathf.FloorToInt(uv.x * textureWidth);
        int y = Mathf.FloorToInt(uv.y * textureHeight);
        if (x + decalWidth > textureWidth || y + decalHeight > textureHeight)
        {
            return;
        }

        Color[] mainColors = mainTexture.GetPixels(x, y, decalWidth, decalHeight);


        Texture2D scaledDecal = new Texture2D(decalWidth, decalHeight);
        Color[] decalColors = decalTexture.GetPixels();

        for (int i = 0; i < decalHeight; i++)
        {
            for (int j = 0; j < decalWidth; j++)
            {
                int scaledX = (int)((float)j / decalWidth * decalTexture.width);
                int scaledY = (int)((float)i / decalHeight * decalTexture.height);
                int index = scaledY * decalTexture.width + scaledX;
                if (decalColors[index].a > 0)
                    mainColors[i * decalWidth + j] = decalColors[index];
            }
        }

        scaledDecal.SetPixels(mainColors);
        scaledDecal.Apply();

        mainTexture.SetPixels(x, y, decalWidth, decalHeight, scaledDecal.GetPixels());

        mainTexture.Apply();
    }
}