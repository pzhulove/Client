
using UnityEngine;
using System.Collections;

public enum SkillSenceEditMode
{
    Normal = 0,
    HurtBoxEdit,
    DefBoxEdit,
    EffectEdit,
    EntityEdit
};

//[ExecuteInEditMode]
public class DSkillDataEditorDrawer : MonoBehaviour {
#if UNITY_EDITOR
    public static readonly float Blocks_EditWidth = 0.15f;
    public static readonly float Blocks_OffZ = 0.0f;
    public static readonly float Camera_ViewSize = 6;
       
    public enum BlocksEditMode
    {
        Null = 0,
        Center,
        LeftTop,
        MidTop,
        RightTop,
        RightMid,
        RightBottom,
        MidBottom,
        LeftBottom,
        LeftMid
    }

      

    [HideInInspector]
    public HurtDecisionBox activeHurtBoxes;

    [HideInInspector]
    public DefenceDecisionBox activeDefBoxes;

    [HideInInspector]
    public System.Object      currentEditObject;

    [HideInInspector]
    public BlocksEditMode editBoxesMode = BlocksEditMode.Null;

    [HideInInspector]
    public SkillSenceEditMode skilleditmode = SkillSenceEditMode.Normal;

    [HideInInspector]
    public DSkillData   _skillData;

    [HideInInspector]
    public bool _play;

    public DSkillData skillData
    {
        get { return _skillData; }
        set
        {
            if(_skillData != value)
            {
                _skillData = value;
            }
        }
    }

    [HideInInspector]
	// Use this for initialization
	void Start () {

        
	}
	
	// Update is called once per frame
	void Update () {
	   
	}


    void OnDrawGizmos()
    {
        
        if(skillData == null || DSkillData.root == null)
        {
            return;
        }

        float zorder = Blocks_OffZ;

       
        Gizmos.matrix = DSkillData.actor.transform.localToWorldMatrix;

        //HurtBoxes
        if (DSkillData.CheckVFliter(DSkillData.vfliter,(VisiableFliter.HurtBox | VisiableFliter.DefBox)))
        {
            if(DSkillData.CheckVFliter(DSkillData.vfliter,VisiableFliter.HurtBox) && activeHurtBoxes != null)
            foreach (ShapeBox HurtBox in activeHurtBoxes.boxs)
            {
                bool bInEdit = (currentEditObject == HurtBox);
                Gizmos.color = (skilleditmode == SkillSenceEditMode.HurtBoxEdit) ? Color.red : Color.magenta;
                
                Vector3 BoxPosition;

                BoxPosition = new Vector3(0, 0, 0);//getPosition(hurtBox.bodyPart);
                BoxPosition += new Vector3(HurtBox.center.x, HurtBox.center.y, zorder);
                
                Vector3 BoxSize = new Vector3(HurtBox.size.x, HurtBox.size.y, 0);
                Gizmos.DrawWireCube(BoxPosition, BoxSize);
                
                if(bInEdit)
                {
                    DrawBoxSelected(BoxPosition, BoxSize);
                }
            }

            if (DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.DefBox)
                && activeDefBoxes != null)
                foreach (ShapeBox DefBox in activeDefBoxes.boxs)
            {
                bool bInEdit = (currentEditObject == DefBox);
                Gizmos.color = (skilleditmode == SkillSenceEditMode.DefBoxEdit) ? Color.cyan : Color.green;

                Vector3 BoxPosition;

                BoxPosition = new Vector3(0, 0, 0);
                BoxPosition += new Vector3(DefBox.center.x, DefBox.center.y, zorder);

                Vector3 BoxSize = new Vector3(DefBox.size.x, DefBox.size.y, 0);
                Gizmos.DrawWireCube(BoxPosition, BoxSize);

                if (bInEdit)
                {
                    DrawBoxSelected(BoxPosition, BoxSize);
                }
            }

            if (
                activeHurtBoxes != null
                &&
                activeHurtBoxes.boxs.Length > 0
                &&
                DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.HurtBox) 
                )
            {
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(10, 0, activeHurtBoxes.zDim));
            }

        }
    }
      

    public void DrawBoxSelected(Vector3 BoxPosition,Vector3 BoxSize)
    {
        Vector3 lefttop, rightbottom, leftbottom, righttop;
        lefttop = BoxPosition + Vector3.Scale(BoxSize, new Vector3(-0.5f, 0.5f, 0));
        rightbottom = BoxPosition + Vector3.Scale(BoxSize, new Vector3(0.5f, -0.5f, 0));
        leftbottom = BoxPosition + Vector3.Scale(BoxSize, new Vector3(-0.5f, -0.5f, 0));
        righttop = BoxPosition + Vector3.Scale(BoxSize, new Vector3(0.5f, 0.5f, 0));

        Vector3 gis = new Vector3(Blocks_EditWidth, Blocks_EditWidth, 0f);

        Gizmos.color = editBoxesMode == BlocksEditMode.Center ? Color.magenta : Color.yellow;
        Gizmos.DrawCube(BoxPosition, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.LeftTop ? Color.magenta : Color.yellow;
        Gizmos.DrawCube(lefttop, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.MidTop ? Color.magenta : Color.yellow;
        Gizmos.DrawCube((lefttop + righttop) / 2, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.RightTop ? Color.magenta : Color.yellow;
        Gizmos.DrawCube(righttop, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.RightMid ? Color.magenta : Color.yellow;
        Gizmos.DrawCube((righttop + rightbottom) / 2, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.RightBottom ? Color.magenta : Color.yellow;
        Gizmos.DrawCube(rightbottom, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.MidBottom ? Color.magenta : Color.yellow;
        Gizmos.DrawCube((rightbottom + leftbottom) / 2, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.LeftBottom ? Color.magenta : Color.yellow;
        Gizmos.DrawCube(leftbottom, gis);

        Gizmos.color = editBoxesMode == BlocksEditMode.LeftMid ? Color.magenta : Color.yellow;
        Gizmos.DrawCube((leftbottom + lefttop) / 2, gis);
    }
    public int lineCount = 100;
    public float radius = 3.0f;

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

    public static void DrawWireCubeWithGLLINES(Vector3 min,Vector3 max,Color color)
    {
        GL.Color(color);

        GL.Vertex3(min.x, min.y, min.z);
        GL.Vertex3(min.x, max.y, min.z);

        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex3(max.x, max.y, min.z);

        GL.Vertex3(max.x, max.y, min.z);
        GL.Vertex3(max.x, min.y, min.z);

        GL.Vertex3(max.x, min.y, min.z);
        GL.Vertex3(min.x, min.y, min.z);
    }

    public static void DrawCubeWithGLQUADS(Vector3 min, Vector3 max, Color color)
    {
        GL.Color(color);
        GL.Vertex3(min.x, min.y, min.z);
        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex3(max.x, max.y, min.z);
        GL.Vertex3(max.x, min.y, min.z);
    }
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        return;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        DrawWireCubeWithGLLINES(new Vector3(-2, -2), new Vector3(2, 2), Color.green);
        GL.End();

        GL.Begin(GL.QUADS);
        DrawCubeWithGLQUADS(new Vector3(-1,-1),new Vector3(1,1),new Color(0,1,0,0.5f));
        GL.End();

        GL.PopMatrix();
    }
#endif
}