using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;


//[ExecuteInEditMode]
[ExecuteAlways]
[AddComponentMenu("UI/Effects/Extensions/Gray")]
public class UIGray : MonoBehaviour {

	public float grayLevel = 1;
    public Color grayFontColor = Color.gray;
    public float alpha = 1.0f;
    public bool bEnabled2Text = true;
    readonly string SHADER_NAME = "UI/DefaultGray";
    readonly string SHADER_ETC_NAME = "UI/DefaultGrayETC1";

    Image[] images = null;
	Dictionary<object, Material> backup = new Dictionary<object, Material>();
    Dictionary<object, Color> Textbackup = new Dictionary<object, Color>();

    List<Material> matBackup = new List<Material>();
    Text[] texts = null;

    string m_Property__AlphaTex = "_AlphaTex";
    string m_Property__GrayScale = "_GrayScale";
    string m_Property__Color = "_Color";

//     void Start()
//     {
//         int m_Property__AlphaTex = Shader.PropertyToID("_AlphaTex");
//         int m_Property__GrayScale = Shader.PropertyToID("_GrayScale");
//         int m_Property__Color = Shader.PropertyToID("_Color");
//    
    void OnEnable()
	{
        // backup.Clear();
        // Textbackup.Clear();

        images = transform.gameObject.GetComponentsInChildren<Image>(true);
        if (bEnabled2Text)
        {
            texts = transform.gameObject.GetComponentsInChildren<Text>(true);
        }
        SetGray();

    }

    public void SetEnable(bool enabled)
    {
        if(enabled)
        {
            OnEnable();
        }
        else
        {
            Restore();
        }
    }

    public void Refresh()
    {
        if (enabled)
        {
            OnEnable();
        }
        else
        {
            Restore();
        }
    }

    public static void Refresh(UIGray gray)
    {
        if(null == gray)
        {
            return;
        }

        if(gray.enabled)
        {
            gray.OnEnable();
        }
        else
        {
            gray.Restore();
        }
    }

    void OnDisable()
	{
        Restore();
	}

	public void SetGray()
	{
		if (images != null)
        {
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                Texture alphaTex = null;
                if (image.material != null)
                {
                    if(null != image.material.shader 
                        && (image.material.shader.name == SHADER_ETC_NAME || image.material.shader.name == SHADER_NAME))
                    {
                        continue;
                    }
                    if(backup.ContainsKey(image))
                    {
                        backup[image] = image.material;
                    }
                    else
                    {
                        backup.Add(image, image.material);
                    }

                    if(image.material.shader.name == "UI/DefaultETC1-Custom")
                    {
                        if(image.material.HasProperty(m_Property__AlphaTex))
                            alphaTex = image.material.GetTexture(m_Property__AlphaTex);
                    }
                }

                Material grayMaterial = null;
                if (null != alphaTex)
                {
                    grayMaterial = GeMaterialPool.instance.CreateMaterialInstance(SHADER_ETC_NAME);
                    grayMaterial.name = SHADER_ETC_NAME;
                }
                else
                {
                    grayMaterial = GeMaterialPool.instance.CreateMaterialInstance(SHADER_NAME);
                    grayMaterial.name = SHADER_NAME;
                }

                if (grayMaterial != null)
                {
                    grayMaterial.SetFloat(m_Property__GrayScale, grayLevel);
                    grayMaterial.SetColor(m_Property__Color, new Color(1, 1, 1, alpha));
                    image.material = grayMaterial;
                    image.canvasRenderer.SetAlphaTexture(alphaTex);
                    matBackup.Add(grayMaterial);
                }
            }
        }

        if(texts != null && bEnabled2Text)
        {
            for (int i = 0; i < texts.Length; ++i)
            {
                var text = texts[i];
                if(null != text)
                {
                    if(Textbackup.ContainsKey(text))
                    {
                        if (text.color == grayFontColor)
                        {
                            continue;
                        }

                        Textbackup[text] = text.color;
                    }
                    else
                    {
                        Textbackup.Add(text, text.color);
                    }

                    text.color = grayFontColor;

                    Outline outline = text.GetComponent<Outline>();
                    if(outline != null)
                    {
                        outline.enabled = false;
                    }

                    NicerOutline niceroutline = text.GetComponent<NicerOutline>();
                    if (niceroutline != null)
                    {
                        niceroutline.enabled = false;
                    }
                }
            }
        }
	}

    public void ResetMaterial()
    {
        // backup.Clear();

        if (images != null)
        {
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                if (image.material != null)
                {
                    if (null != image.material.shader
                        && (image.material.shader.name == SHADER_ETC_NAME || image.material.shader.name == SHADER_NAME))
                    {
                        continue;
                    }

                    if(backup.ContainsKey(image))
                    {
                        backup[image] = image.material;
                    }
                    else
                    {
                        backup.Add(image, image.material);
                    }

                    if(image.material.HasProperty(m_Property__AlphaTex))
                        image.canvasRenderer.SetAlphaTexture(image.material.GetTexture(m_Property__AlphaTex));
                }
            }
        }
    }

    public void ResetMaterial(Image image)
    {
        if (image != null)
        {
            if (image.material != null)
            {
                backup.Remove(image);
                backup.Add(image, image.material);
            }
        }
    }

    public void SetImageAlpha(Image srcImage, Image destImage)
    {
        if(null == srcImage)
        {
            return;
        }

        if(backup.ContainsKey(srcImage))
        {
            if (backup[srcImage].HasProperty(m_Property__AlphaTex))
            {
                Texture alphaTexture = backup[srcImage].GetTexture(m_Property__AlphaTex);
                destImage.canvasRenderer.SetAlphaTexture(alphaTexture);
            }
        }
    }

	public void Restore()
	{
		if (images != null)
        {
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
				if (image != null)
				{
                    if (backup.ContainsKey(image))
					{
                        image.material = backup[image];
                    }
					else
					{
                        if(null != image.material)
                        {
                            if(image.material.name == "UI/DefaultGray"
                                || image.material.name == "UI/DefaultGrayETC1")
                            {
                                image.material = null;
                            }
                        }
					}
				}
                
            }
        }

        if(texts != null)
        {
            for (int i = 0; i < texts.Length; ++i)
            {
                var text = texts[i];
				if (text != null)
				{
                    if (Textbackup.ContainsKey(text))
                    {
                        text.color = Textbackup[text];
                    }
					else
                    {
                        text.color = Color.white;
                    }

                    Outline outline = text.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = true;
                    }

                    NicerOutline niceroutline = text.GetComponent<NicerOutline>();
                    if (niceroutline != null)
                    {
                        niceroutline.enabled = true;
                    }
                }
            }
        }

        for (int i = 0, icnt = matBackup.Count; i < icnt; ++i)
            GeMaterialPool.instance.RecycleMaterialInstance(matBackup[i].name, matBackup[i]);

        matBackup.Clear();
        backup.Clear ();
        Textbackup.Clear();
    }
}
