using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ETCButton))]
public class ETCButtonInspector : Editor {

	public string[] unityAxes;
	
	void OnEnable(){
		var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
		SerializedObject obj = new SerializedObject(inputManager);
		SerializedProperty axisArray = obj.FindProperty("m_Axes");
		if (axisArray.arraySize > 0){
			unityAxes = new string[axisArray.arraySize];
			for( int i = 0; i < axisArray.arraySize; ++i ){
				var axis = axisArray.GetArrayElementAtIndex(i);
				unityAxes[i] = axis.FindPropertyRelative("m_Name").stringValue;
			}
		}
		
	}

	public override void OnInspectorGUI(){
		
		ETCButton t = (ETCButton)target;
		
		EditorGUILayout.Space();

		t.gameObject.name = EditorGUILayout.TextField("按钮名称",t.gameObject.name);
		t.axis.name = t.gameObject.name;

		t.activated = ETCGuiTools.Toggle("是否激活",t.activated,true);
		t.visible = ETCGuiTools.Toggle("是否可视",t.visible,true);

		EditorGUILayout.Space();
		t.useFixedUpdate = ETCGuiTools.Toggle("使用 Fixed Update",t.useFixedUpdate,true);
		t.isUnregisterAtDisable = ETCGuiTools.Toggle("Unregister at disabling time",t.isUnregisterAtDisable,true);

		#region Position & Size
		t.showPSInspector = ETCGuiTools.BeginFoldOut( "位置大小",t.showPSInspector);
		if (t.showPSInspector){
			ETCGuiTools.BeginGroup();{
				// Anchor
				t.anchor = (ETCBase.RectAnchor)EditorGUILayout.EnumPopup( "位置",t.anchor);
				if (t.anchor != ETCBase.RectAnchor.UserDefined){
					t.anchorOffet = EditorGUILayout.Vector2Field("偏移",t.anchorOffet);
				}

				EditorGUILayout.Space();
				
				// Area sprite ratio
				if (t.GetComponent<Image>().sprite != null){
					Rect rect =  t.GetComponent<Image>().sprite.rect;
					float ratio = rect.width / rect.height;
					
					// Area Size
					if (ratio>=1){
						float s = EditorGUILayout.FloatField("大小", t.rectTransform().rect.width);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
					}
					else{
						float s = EditorGUILayout.FloatField("大小", t.rectTransform().rect.height);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
					}
				}
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Behaviour
		t.showBehaviourInspector = ETCGuiTools.BeginFoldOut( "行为",t.showBehaviourInspector);
		if (t.showBehaviourInspector){
			ETCGuiTools.BeginGroup();{

				EditorGUILayout.Space();
				ETCGuiTools.BeginGroup(5);{
					t.enableKeySimulation = ETCGuiTools.Toggle("Enable Unity axes",t.enableKeySimulation,true);
					if (t.enableKeySimulation){
						t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow Unity axes on standalone",t.allowSimulationStandalone,true);
						t.visibleOnStandalone = ETCGuiTools.Toggle("Force visible",t.visibleOnStandalone,true);
					}
				}ETCGuiTools.EndGroup();

				#region General propertie
				EditorGUI.indentLevel++;
				t.axis.showGeneralInspector = EditorGUILayout.Foldout(t.axis.showGeneralInspector,"General setting");
				if (t.axis.showGeneralInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						t.isSwipeIn = ETCGuiTools.Toggle("划入",t.isSwipeIn,true);
						t.isSwipeOut = ETCGuiTools.Toggle("划出",t.isSwipeOut,true);
                        if (t.isSwipeOut)
                        {
                            t.dragMode = (ETCButton.DragDivMode)EditorGUILayout.EnumPopup("拖拽模式", t.dragMode);
                        }

                        t.axis.isValueOverTime = ETCGuiTools.Toggle("Value over the time",t.axis.isValueOverTime,true);
						if (t.axis.isValueOverTime){

							ETCGuiTools.BeginGroup(5);{
								t.axis.overTimeStep = EditorGUILayout.FloatField("Step",t.axis.overTimeStep);
								t.axis.maxOverTimeValue = EditorGUILayout.FloatField("Max value",t.axis.maxOverTimeValue);
							}ETCGuiTools.EndGroup();

						}
						t.axis.speed = EditorGUILayout.FloatField("Value",t.axis.speed);

						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				EditorGUI.indentLevel--;
				#endregion

				#region Direct Action
				EditorGUI.indentLevel++;
				t.axis.showDirectInspector = EditorGUILayout.Foldout(t.axis.showDirectInspector,"Direction ation");
				if (t.axis.showDirectInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						t.axis.autoLinkTagPlayer = EditorGUILayout.ToggleLeft("Auto link on tag",t.axis.autoLinkTagPlayer, GUILayout.Width(200));
						if (t.axis.autoLinkTagPlayer){
							t.axis.autoTag = EditorGUILayout.TagField("",t.axis.autoTag);
						}
						else{
							t.axis.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",t.axis.directTransform,typeof(Transform),true);
						}

						EditorGUILayout.Space();
	
						t.axis.actionOn = (ETCAxis.ActionOn)EditorGUILayout.EnumPopup("Action on",t.axis.actionOn);

						t.axis.directAction = (ETCAxis.DirectAction ) EditorGUILayout.EnumPopup( "Action",t.axis.directAction);

						if (t.axis.directAction != ETCAxis.DirectAction.Jump){
							t.axis.axisInfluenced = (ETCAxis.AxisInfluenced) EditorGUILayout.EnumPopup("Affected axis",t.axis.axisInfluenced);
						}
						else{
							EditorGUILayout.HelpBox("Required character controller", MessageType.Info);
							t.axis.gravity = EditorGUILayout.FloatField("Gravity",t.axis.gravity);
						}
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				EditorGUI.indentLevel--;
				#endregion

				#region Unity axis
				EditorGUI.indentLevel++;
				t.axis.showSimulatinInspector = EditorGUILayout.Foldout(t.axis.showSimulatinInspector,"Unity axes");
				if (t.axis.showSimulatinInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;
						int index = System.Array.IndexOf(unityAxes,t.axis.unityAxis );
						int tmpIndex = EditorGUILayout.Popup(index,unityAxes);
						if (tmpIndex != index){
							t.axis.unityAxis = unityAxes[tmpIndex];
						}
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
					
				}

				EditorGUI.indentLevel--;
                #endregion
            }
            ETCGuiTools.EndGroup();
		}
		#endregion

		#region Sprite
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "按钮图片",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{

				// Normal state				
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck ();
				t.normalSprite = (Sprite)EditorGUILayout.ObjectField("默认图片",t.normalSprite,typeof(Sprite),true,GUILayout.MinWidth(100));

				EditorGUILayout.BeginVertical(GUILayout.Width(50));
                {
                    t.normalColor = EditorGUILayout.ColorField("", t.normalColor, GUILayout.Width(50));
                    t.normalDisableColor = EditorGUILayout.ColorField("", t.normalDisableColor, GUILayout.Width(50));
                }
                EditorGUILayout.EndVertical();

				if (EditorGUI.EndChangeCheck ()) {
					t.GetComponent<Image>().sprite = t.normalSprite;
					t.GetComponent<Image>().color = t.normalColor;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if ( t.normalSprite){
					Rect spriteRect = new Rect( t.normalSprite.rect.x/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.y/ t.normalSprite.texture.height,
					                           t.normalSprite.rect.width/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.height/ t.normalSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.normalSprite.texture,Color.white);
				}		

				// Press state
				EditorGUILayout.BeginHorizontal();
				t.pressedSprite = (Sprite)EditorGUILayout.ObjectField("按下图片",t.pressedSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.pressedColor = EditorGUILayout.ColorField("",t.pressedColor,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (t.pressedSprite){
					Rect spriteRect = new Rect( t.pressedSprite.rect.x/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.y/ t.pressedSprite.texture.height,
					                      t.pressedSprite.rect.width/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.height/ t.pressedSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.pressedSprite.texture,Color.white);
				}

				// fg state
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck ();
				t.fgSprite = (Sprite)EditorGUILayout.ObjectField("前景图片",t.fgSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				EditorGUILayout.BeginVertical(GUILayout.Width(50));
				t.fgColor = EditorGUILayout.ColorField("",t.fgColor,GUILayout.Width(50));
				t.fgCoolDownColor = EditorGUILayout.ColorField("",t.fgCoolDownColor,GUILayout.Width(50));
                EditorGUILayout.EndVertical();
				if (EditorGUI.EndChangeCheck ()) {
                    t.fgImage.sprite = t.fgSprite;
                    t.fgImage.color = t.fgColor;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (t.fgSprite){
					Rect spriteRect = new Rect( t.fgSprite.rect.x/ t.fgSprite.texture.width,
					                      t.fgSprite.rect.y/ t.fgSprite.texture.height,
					                      t.fgSprite.rect.width/ t.fgSprite.texture.width,
					                      t.fgSprite.rect.height/ t.fgSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.fgSprite.texture,Color.white);
				}

                // Cooldown state
                EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck ();
				t.coolDownSprite = (Sprite)EditorGUILayout.ObjectField("CD图片",t.coolDownSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.coolDownColor = EditorGUILayout.ColorField("",t.coolDownColor,GUILayout.Width(50));
				if (EditorGUI.EndChangeCheck ()) {
					t.coolDownImage.sprite = t.coolDownSprite;
					t.coolDownImage.color = t.coolDownColor;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if ( t.coolDownSprite){
					Rect spriteRect = new Rect( t.coolDownSprite.rect.x / t.coolDownSprite.texture.width,
					                           t.coolDownSprite.rect.y / t.coolDownSprite.texture.height,
					                           t.coolDownSprite.rect.width / t.coolDownSprite.texture.width,
					                           t.coolDownSprite.rect.height / t.coolDownSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.coolDownSprite.texture,Color.white);
				}

			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Events
		t.showEventInspector = ETCGuiTools.BeginFoldOut( "事件",t.showEventInspector);
        if (t.showEventInspector)
        {
            ETCGuiTools.BeginGroup();
            {

                serializedObject.Update();
                SerializedProperty down = serializedObject.FindProperty("onDown");
                EditorGUILayout.PropertyField(down, true, null);
                serializedObject.ApplyModifiedProperties();

                serializedObject.Update();
                SerializedProperty press = serializedObject.FindProperty("onPressed");
                EditorGUILayout.PropertyField(press, true, null);
                serializedObject.ApplyModifiedProperties();

                serializedObject.Update();
                SerializedProperty pressTime = serializedObject.FindProperty("onPressedValue");
                EditorGUILayout.PropertyField(pressTime, true, null);
                serializedObject.ApplyModifiedProperties();

                serializedObject.Update();
                SerializedProperty up = serializedObject.FindProperty("onUp");
                EditorGUILayout.PropertyField(up, true, null);
                serializedObject.ApplyModifiedProperties();

            }
            ETCGuiTools.EndGroup();

            ETCGuiTools.BeginGroup();
            {
                // on cool down start
                serializedObject.Update();
                SerializedProperty start = serializedObject.FindProperty("onCoolDownStart");
                EditorGUILayout.PropertyField(start, true, null);
                serializedObject.ApplyModifiedProperties();

                // on cool down end
                serializedObject.Update();
                SerializedProperty down = serializedObject.FindProperty("onCoolDownEnd");
                EditorGUILayout.PropertyField(down, true, null);
                serializedObject.ApplyModifiedProperties();

                // on cool down step
                serializedObject.Update();
                SerializedProperty step = serializedObject.FindProperty("onCoolDownStep");
                EditorGUILayout.PropertyField(step, true, null);
                serializedObject.ApplyModifiedProperties();
            }
            ETCGuiTools.EndGroup();

            ETCGuiTools.BeginGroup();
            {
                serializedObject.Update();
                SerializedProperty up = serializedObject.FindProperty("onDragOutUp");
                EditorGUILayout.PropertyField(up, true, null);
                serializedObject.ApplyModifiedProperties();

                serializedObject.Update();
                SerializedProperty down = serializedObject.FindProperty("onDragOutDown");
                EditorGUILayout.PropertyField(down, true, null);
                serializedObject.ApplyModifiedProperties();
            }
            ETCGuiTools.EndGroup();

        }

		#endregion

		if (GUI.changed){
			EditorUtility.SetDirty(t);
		}

		if (t.anchor != ETCBase.RectAnchor.UserDefined){
			t.SetAnchorPosition();
		}
	}
}
