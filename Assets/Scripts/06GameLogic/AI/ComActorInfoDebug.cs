#if UNITY_EDITOR
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ComActorInfoDebug : MonoBehaviour
{
    BeActor mActor;
    public BeActor actor
    {
        get { return mActor; }
        set { mActor = value; } 
    }

    private GUIStyle fontStyle;

    static public bool bShowSight = false;
    static public bool bShowDebugInfo = false;

    static public bool bShowLevelDetail = false;
    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }
    
    private void DrawGizmos()
    {
        if(actor == null)
            return;
        var ai = actor.aiManager as BeActorAIManager;
        if (ai == null)
            return;
        DrawSight(ai);
        DrawDestination(ai);
        DrawTarget(ai);
        DrawInfo(ai);
    }

    private void DrawInfo(BeActorAIManager ai)
    {
        if(!bShowDebugInfo)
            return;
        
        if (ai == null)
            return;
            
        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        if (ai.currentCmd != null)
        {
            stringBuilder.AppendLine("当前指令：" + ai.currentCmd.GetType());
            var walkcmd = ai.currentCmd as BeAIWalkCommand;
            if (walkcmd != null)
            {
                stringBuilder.AppendLine("寻路模式：" + walkcmd.destinationType);
            }
        }
        Handles.Label(transform.position + Vector3.up * 2, stringBuilder.ToString());
        StringBuilderCache.Release(stringBuilder);
    }

    private void DrawTarget(BeActorAIManager ai)
    {
        if (ai.aiTarget != null)
        {
            Handles.color = new Color(1f, 0.0f, 0.0f, 0.5f);
            Handles.ConeHandleCap(0, ai.aiTarget.GetPosition().vector3 + 2*Vector3.up, Quaternion.LookRotation(Vector3.down), 0.5f, EventType.Repaint);
        }

        if (ai.followTarget != null)
        {
            Handles.color = new Color(0.0f, 1f, 0.0f, 0.5f);
            Handles.ConeHandleCap(0, ai.followTarget.GetPosition().vector3 + 2*Vector3.up, Quaternion.LookRotation(Vector3.down), 0.5f, EventType.Repaint);
        }
    }
    private void DrawSight(BeActorAIManager ai)
    {
        if (bShowSight)
        {
            if (ai.aiTarget == null)
            {
                Handles.color = new Color(1, 1, 1, 0.1f);
                Handles.DrawSolidDisc(transform.position, Vector3.up, ai.sight / 1000.0f);
            }
            else
            {
                Handles.color = new Color(1, 0, 0, 0.1f);
                Handles.DrawSolidDisc(transform.position, Vector3.up, ai.chaseSight / 1000.0f);
            }
        }
    }

    private void DrawDestination(BeActorAIManager ai)
    {
        var cmd = ai.currentCmd;
        if(cmd == null)
            return;

        var walkCmd = cmd as BeAIWalkCommand;

        if(walkCmd == null)
            return;
        if (walkCmd.Steps != null)
        {
            Vector3 fromPos = walkCmd.LastPos.vector3;
        
            for (int i = 0; i < walkCmd.Steps.Count; i++)
            {
                BeAIManager.MoveDir curDir = (BeAIManager.MoveDir) walkCmd.Steps[i];
                var delta = BeAIWalkCommand.grid.vector2;
                var deltaV3 = new Vector3(delta.x, 0, delta.y);
                var offset = GetDirOffset(curDir).normalized;
                offset.Scale(deltaV3);
                var toPos = fromPos + offset;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(fromPos, toPos);
                fromPos = toPos;
            }    
        }
        
        Handles.color = Color.white;
        Handles.DrawDottedLine(actor.GetPosition().vector3, walkCmd.targetPos.vector3, 2f);
    }

    private Vector3 GetDirOffset(BeAIManager.MoveDir dir)
    {
        switch (dir)
        {
            case BeAIManager.MoveDir.RIGHT:
                return Vector3.right;
            case BeAIManager.MoveDir.LEFT:
                return Vector3.left;
            case BeAIManager.MoveDir.TOP:
                return Vector3.forward;
            case BeAIManager.MoveDir.DOWN:
                return Vector3.back;
            case BeAIManager.MoveDir.RIGHT_TOP:
                return Vector3.right + Vector3.forward;
            case BeAIManager.MoveDir.LEFT_TOP:
                return Vector3.left + Vector3.forward;
            case BeAIManager.MoveDir.RIGHT_DOWN:
                return Vector3.right + Vector3.back;
            case BeAIManager.MoveDir.LEFT_DOWN:
                return Vector3.left + Vector3.back;
            case BeAIManager.MoveDir.COUNT:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}

#endif
