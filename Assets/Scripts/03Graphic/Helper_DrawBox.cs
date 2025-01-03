using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Helper_DrawBox : MonoBehaviour {

    public BDEntityActionFrameData frameData = null;

    public static DBox hitBox;
    float scale = 1.0f;
	float zdimScale = 1.0f;

    float scalex = 1.0f;

    public void SetFrameData(BDEntityActionFrameData data, float scale = 1.0f, float zdimScale=1.0f,float scalex = 1.0f)
    {
        frameData = data;
        this.scale = scale;
		this.zdimScale = zdimScale;
        this.scalex = scalex;
    }

    float zDimMinX = float.MaxValue;
    float zDimMaxX = float.MinValue;

    public void DrawBox(int index)
    {

        if (index == 2 && hitBox.IsValide())
        {
            DrawCubeWithGLQUADS(new Vector3(hitBox._min.fx, hitBox._min.fy), new Vector3(hitBox._max.fx, hitBox._max.fy), new Color(0, 1, 0, 0.5f));
            return;
        }

        if (frameData != null)
        {
            //for(int i=0; i<2; ++i)
            {
                int i = index;
                BDDBoxData data = i == 0 ? frameData.pDefenseData : frameData.pAttackData;
                if (data == null)
                    return;

                for (int j = 0; j < data.vBox.Count; ++j)
                {
                    DBoxImp box = data.vBox[j];
                    //DrawWireCubeWithGLLINES(new Vector3(box.vBox._min.x, box.vBox._min.y,), new Vector3(box.vBox._max.x, box.vBox._max.y), new Color(1, 0, 0, 0.5f))

                    if (i == 0)
                        DrawWireCubeWithGLLINES(new Vector3(box.vBox._min.fx * scale * scalex, box.vBox._min.fy * scale), new Vector3(box.vBox._max.fx * scale * scalex, box.vBox._max.fy * scale), new Color(0, 1, 0, 0.5f));
                    else if (i == 1)
                    {
                        if (zDimMaxX < box.vBox._max.fx)
                        {
                            zDimMaxX = box.vBox._max.fx;
                        }

                        if (zDimMinX > box.vBox._min.fx)
                        {
                            zDimMinX = box.vBox._min.fx;
                        }

                        if (DBoxConfig.b2D)
                        {
                            DrawWireCubeWithGLLINES(new Vector3(box.vBox._min.fx * scale * scalex, box.vBox._min.fy * scale), new Vector3(box.vBox._max.fx * scale * scalex, box.vBox._max.fy * scale), new Color(1, 0, 0, 0.5f));
                        }
                        else
                        {
							float zDim = frameData.pAttackData.zDimInt.scalar * Global.Settings.zDimFactor.single * zdimScale;
                            DrawWireCubeWithGLLINES(new Vector3(box.vBox._min.fx, box.vBox._min.fy, -zDim), new Vector3(box.vBox._max.fx, box.vBox._max.fy, zDim), new Color(1, 0, 0, 0.5f));

                        }
                    }
                }

                if (i == 3)
                {
                    if ( DBoxConfig.b2D && (zDimMinX <= zDimMaxX) )
                    {
                        Vector3 postition = gameObject.transform.position;
						float zDim = frameData.pAttackData.zDimInt.scalar * Global.Settings.zDimFactor.single * zdimScale;
                        DrawCubeWithGLQUADS_Y(new Vector3(zDimMinX * scale * scalex, 0.01f - postition.y,- zDim), new Vector3(zDimMaxX * scale * scalex, 0.01f - postition.y,zDim), new Color(1, 0, 1, 0.3f));
                    }
                }
            }



        }
    }

    public void Update()
    {

    }


    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = AssetShaderLoader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public static void DrawWireCubeWithGLLINES(Vector3 min, Vector3 max, Color color)
    {
        GL.Color(color);

        if(min.z == max.z)
        {
            GL.Vertex3(min.x, min.y, min.z);
            GL.Vertex3(min.x, max.y, min.z);

            GL.Vertex3(min.x, max.y, min.z);
            GL.Vertex3(max.x, max.y, min.z);

            GL.Vertex3(max.x, max.y, min.z);
            GL.Vertex3(max.x, min.y, min.z);

            GL.Vertex3(max.x, min.y, min.z);
            GL.Vertex3(min.x, min.y, min.z);
        }
        else
        {
            GL.Vertex3(min.x, max.y, min.z);
            GL.Vertex3(max.x, max.y, min.z);

            GL.Vertex3(max.x, max.y, min.z);
            GL.Vertex3(max.x, max.y, max.z);

            GL.Vertex3(max.x, max.y, max.z);
            GL.Vertex3(min.x, max.y, max.z);

            GL.Vertex3(min.x, max.y, max.z);
            GL.Vertex3(min.x, max.y, min.z);

            GL.Vertex3(min.x, min.y, min.z);
            GL.Vertex3(max.x, min.y, min.z);

            GL.Vertex3(max.x, min.y, min.z);
            GL.Vertex3(max.x, min.y, max.z);

            GL.Vertex3(max.x, min.y, max.z);
            GL.Vertex3(min.x, min.y, max.z);

            GL.Vertex3(min.x, min.y, max.z);
            GL.Vertex3(min.x, min.y, min.z);


            GL.Vertex3(min.x, min.y, min.z);
            GL.Vertex3(min.x, max.y, min.z);

            GL.Vertex3(max.x, min.y, max.z);
            GL.Vertex3(max.x, max.y, max.z);

            GL.Vertex3(max.x, min.y, min.z);
            GL.Vertex3(max.x, max.y, min.z);

            GL.Vertex3(min.x, min.y, max.z);
            GL.Vertex3(min.x, max.y, max.z);
        }
       
    }

    public static void DrawCubeWithGLQUADS(Vector3 min, Vector3 max, Color color)
    {
        GL.Color(color);
        GL.Vertex3(min.x, min.y, min.z);
        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex3(max.x, max.y, min.z);
        GL.Vertex3(max.x, min.y, min.z);
    }

    public static void DrawCubeWithGLQUADS_Y(Vector3 min, Vector3 max, Color color)
    {
        GL.Color(color);
        GL.Vertex3(min.x, min.y, min.z);
        GL.Vertex3(min.x, min.y, max.z);
        GL.Vertex3(max.x, min.y, max.z);
        GL.Vertex3(max.x, min.y, min.z);
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if(Camera.current != Camera.main && Camera.current.name != "SceneCamera")
            return;
        
        zDimMinX = float.MaxValue;
        zDimMaxX = float.MinValue;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        DrawBox(0);//防御框
        GL.End();

        GL.Begin(GL.LINES);
        DrawBox(1);//攻击匡
        GL.End();


        //GL.MultMatrix(Matrix4x4.identity);
        GL.Begin(GL.QUADS);
        DrawBox(3);//攻击匡
        GL.End();

        GL.PopMatrix();


        //GL.Begin(GL.QUADS);
        //DrawBox(2);//攻击匡
        //GL.End();
    }
}
