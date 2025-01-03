using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Diagnostics;
using GameClient;
using ProtoTable;


public class DragWindows : MonoBehaviour
{

	enum slideVector { nullVector, up, down, left, right };  
	private Vector2 touchFirst = Vector2.zero; //手指开始按下的位置  
	private Vector2 touchSecond = Vector2.zero; //手指拖动的位置  
	private slideVector currentVector = slideVector.nullVector;//当前滑动方向  
	private float timer;//时间计数器    
	public float offsetTime = 0.2f;//判断的时间间隔   
	public float SlidingDistance = 100f;  

	void OnGUI()
	{  
		if (Event.current.type == EventType.MouseDown)  
			//判断当前手指是按下事件   
		{
            currentVector = slideVector.nullVector;
            touchFirst = Event.current.mousePosition;//记录开始按下的位置  
		}  
		if (Event.current.type == EventType.MouseDrag)  
			//判断当前手指是拖动事件  
		{  
			touchSecond = Event.current.mousePosition;  

			timer += Time.deltaTime;  //计时器  

			if (timer > offsetTime)  
			{  
				touchSecond = Event.current.mousePosition; //记录结束下的位置  
				Vector2 slideDirection = touchFirst - touchSecond;  
				float x = slideDirection.x;  
				float y = slideDirection.y;  

				if (y + SlidingDistance < x && y > -x - SlidingDistance)  
				{

                    if (currentVector == slideVector.left)
                    {
                        return;
                    }

                    //Logger.LogErrorFormat ("Right"); 
                    UIEventSystem.GetInstance ().SendUIEvent (EUIEventID.RightSlip);
					currentVector = slideVector.left;  
				}  
				else if (y > x + SlidingDistance && y < -x - SlidingDistance)  
				{
                    if (currentVector == slideVector.right)
                    {
                        return;
                    }

                    //Logger.LogErrorFormat ("Left"); 
                    UIEventSystem.GetInstance ().SendUIEvent (EUIEventID.LeftSlip);
					currentVector = slideVector.right;  
				}  
				else if (y > x + SlidingDistance && y - SlidingDistance > -x)  
				{
                    if (currentVector == slideVector.up)
                    {
                        return;
                    }

                    //Logger.LogErrorFormat ("Up"); 

                    currentVector = slideVector.up;  
				}  
				else if (y + SlidingDistance < x && y < -x - SlidingDistance)  
				{
                    if (currentVector == slideVector.down)
                    {
                        return;
                    }

                    //Logger.LogErrorFormat ("Down");   

                    currentVector = slideVector.down;  
				}  

				timer = 0;  
				touchFirst = touchSecond;  
			}  
			//if (Event.current.type == EventType.MouseUp)  
			//{//滑动结束    
			//	currentVector = slideVector.nullVector;  
			//}  
		}   // 滑动方法  
	}    
}
