using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Silhouette : MonoBehaviour
{
    [SerializeField]
    public float m_BlurSize = 0.5f;
    [SerializeField]
    public int m_Interator = 2;
    public Material m_MaterialGaussianBlur, m_MaterialSilhouette;
    public int m_Size = 4;
    // Start is called before the first frame update
    void Start()
    {
        m_MaterialGaussianBlur = new Material(Shader.Find("Shadow/GaussianBlur"));
        m_MaterialSilhouette = new Material(Shader.Find("Shadow/Silhouette"));
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {

        m_MaterialGaussianBlur.SetFloat("_BlurSize", m_BlurSize);
        int rtW = sourceTexture.width / m_Size;
        int rtH = sourceTexture.height / m_Size;
        Graphics.ClearRandomWriteTargets();

        RenderTexture rtTempA = RenderTexture.GetTemporary(rtW, rtH, 0, sourceTexture.format);
        rtTempA.filterMode = FilterMode.Bilinear;

        RenderTexture rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0, sourceTexture.format);
        rtTempB.filterMode = FilterMode.Bilinear;


        for (int i = 0; i < m_Interator; i++)
        {
            if (0 == i)
            {
                Graphics.Blit(sourceTexture, rtTempA, m_MaterialGaussianBlur, 0);
                Graphics.Blit(rtTempA, rtTempB, m_MaterialGaussianBlur, 1);
            }
            else
            {

                Graphics.Blit(rtTempB, rtTempA, m_MaterialGaussianBlur, 0);
                Graphics.Blit(rtTempA, rtTempB, m_MaterialGaussianBlur, 1);
            }
        }
        Graphics.Blit(rtTempB, destTexture, m_MaterialSilhouette);
        RenderTexture.ReleaseTemporary(rtTempA);
        RenderTexture.ReleaseTemporary(rtTempB);
    }
}
