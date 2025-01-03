#if UNITY_EDITOR
using UnityEngine; 
using UnityEditor; 
using System.Collections; 
using System.Reflection;  
public static class DEditorCamera { 
	public static Vector3 SkillEditorDefaultCameraPos = new Vector3(1000, 1, 0);

	public static void SetPosition( Vector3 newPosition , SceneView sceneView = null ) { 
		SetEditorCameraValue<Vector3>( "m_Position", newPosition, sceneView );
	}  
	public static void SetRotation( Quaternion newRotation , SceneView sceneView = null ) { 
		SetEditorCameraValue<Quaternion>( "m_Rotation", newRotation, sceneView );
	}
	public static void SetSize( float newSize , SceneView sceneView = null ) { 
		SetEditorCameraValue<float>( "m_Size", newSize, sceneView );
	}
	public static void SetOrthographic( bool newOrthographic , SceneView sceneView = null ) {
        if(sceneView)
        sceneView.orthographic = newOrthographic;
        SetEditorCameraValue_bool("m_Ortho", (newOrthographic == true) ? true : false, sceneView); 
	}  
	public static float GetSize( SceneView sceneView = null ) { 
		return GetEditorCameraValue<float>( "m_Size", sceneView );
	}  
	public static Vector3 GetPosition( SceneView sceneView = null ) { 
		return GetEditorCameraValue<Vector3>( "m_Position", sceneView );
	}  
	public static Quaternion GetRotation( SceneView sceneView = null ) { 
		return GetEditorCameraValue<Quaternion>( "m_Rotation", sceneView );
	}  
	public static bool GetOrthographic( SceneView sceneView = null ) { 
		return GetEditorCameraValue<float>( "m_Ortho", sceneView ) == 1f; 
	}  
	public static SceneView GetActiveSceneView() { 
		if( EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == typeof( SceneView ) ) { 
			return (SceneView)EditorWindow.focusedWindow; }  ArrayList temp = SceneView.sceneViews;  
		return (SceneView)temp[ 0 ]; 
	}  
	static T GetEditorCameraValue<T>( string fieldName , SceneView sceneView = null ) { 
		FieldInfo field = typeof( SceneView ).GetField( fieldName , BindingFlags.Instance | BindingFlags.NonPublic );  
		object animBool = field.GetValue( ( sceneView != null ) ? sceneView : GetActiveSceneView() );  
		FieldInfo field2 = animBool.GetType().GetField( "m_Value" , BindingFlags.Instance | BindingFlags.NonPublic );  
		return (T)field2.GetValue( animBool );
	}  
	static void SetEditorCameraValue<T>( string fieldName , T newValue , SceneView sceneView = null ) { 
		FieldInfo field = typeof( SceneView ).GetField( fieldName , BindingFlags.Instance | BindingFlags.NonPublic );  
		object animBool = field.GetValue( ( sceneView != null ) ? sceneView : GetActiveSceneView() );  
		FieldInfo field2 = animBool.GetType().GetField( "m_Value" , BindingFlags.Instance | BindingFlags.NonPublic );  
		object animBoolValue = field2.GetValue( animBool );

        
        T currentValue = (T)animBoolValue;
		object[] param = new object[ 2 ]; param[ 0 ] = newValue; param[ 1 ] = currentValue;  
		animBool.GetType() .GetMethod( "BeginAnimating" , BindingFlags.Instance | BindingFlags.NonPublic ) .Invoke( animBool, param );
	}

    static void SetEditorCameraValue_bool(string fieldName, bool newValue, SceneView sceneView = null)
    {
        FieldInfo field = typeof(SceneView).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        object animBool = field.GetValue((sceneView != null) ? sceneView : GetActiveSceneView());
        FieldInfo field2 = animBool.GetType().GetField("m_Value", BindingFlags.Instance | BindingFlags.NonPublic);
        float animBoolValue = (float)field2.GetValue(animBool);


        bool currentValue =  animBoolValue > 0.0f;
        object[] param = new object[2]; param[0] = newValue; param[1] = currentValue;
        animBool.GetType().GetMethod("BeginAnimating", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(animBool, param);
    } 
}

#endif