using Tenmove.Runtime;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Tenmove.Editor.Unity
{
    public static partial class Extension
    {
        private class _EditorWindow : ReflectWrapper<EditorWindow>
        {
            public _EditorWindow(EditorWindow instance)
                : base(instance)
            {
            }

            public object m_Parent
            {
                get
                {
                    var field = m_TargetType.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(m_TargetInstance);
                }
            }
        }

        private class _DockArea : ReflectWrapper<object>
        {
            public _DockArea(object instance)
                : base(instance)
            {
            }

            public object Window
            {
                get
                {
                    var property = m_TargetType.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(m_TargetInstance, null);
                }
            }

            public object Parent
            {
                get
                {
                    var property = m_TargetType.GetProperty("parent", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(m_TargetInstance, null);
                }
            }

            public List<EditorWindow> Panes
            {
                get
                {
                    var field = m_TargetType.GetField("m_Panes", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(m_TargetInstance) as List<EditorWindow>;
                }
            }

            public RectOffset borderSize
            {
                get
                {
                    var property = m_TargetType.GetProperty("borderSize", BindingFlags.Instance | BindingFlags.NonPublic);
                    return property.GetValue(m_TargetInstance, null) as RectOffset;
                }
            }

            public object OriginalDragSource
            {
                set
                {
                    var field = m_TargetType.GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic);
                    field.SetValue(null, value);
                }
            }

            public GUIStyle Background
            {
                get
                {
                    var field = m_TargetType.GetField("background", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(m_TargetInstance) as GUIStyle;
                }
            }
        }

        private class _Container : ReflectWrapper<object>
        {
            public _Container(object instance)
                : base(instance)
            {
            }

            public object RootSplitView
            {
                get
                {
                    var property = m_TargetType.GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(m_TargetInstance, null);
                }
            }
        }

        private class _SplitView : ReflectWrapper<object>
        {
            public _SplitView(object instance)
                : base(instance)
            {
            }

            public object DragOver(EditorWindow child, Vector2 screenPoint)
            {
                var method = m_TargetType.GetMethod("DragOver", BindingFlags.Instance | BindingFlags.Public);
                return method.Invoke(m_TargetInstance, new object[] { child, screenPoint });
            }

            public void PerformDrop(EditorWindow child, object dropInfo, Vector2 screenPoint)
            {
                var method = m_TargetType.GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(m_TargetInstance, new object[] { child, dropInfo, screenPoint });
            }
        }

        private class _DropInfo : ReflectWrapper<object>
        {
            public _DropInfo(object instance)
                : base(instance)
            {
            }

            public Rect Rect
            {
                get
                {
                    var field = m_TargetType.GetField("rect", BindingFlags.Instance | BindingFlags.Public);
                    return (Rect)field.GetValue(m_TargetInstance);
                }
            }

            public object Instance
            {
                get { return m_TargetInstance; }
            }
        }

        /// <summary>
        /// Docks the second window to the first window at the given position
        /// </summary>
        public static void DockTo(this EditorWindow dockChild, EditorWindow target, DockSide dockSide)
        {
            var dockPosition = _GetDockSidePosition(target, dockSide);

            _EditorWindow parent = new _EditorWindow(target);
            _EditorWindow child = new _EditorWindow(dockChild);
            _DockArea dockArea = new _DockArea(parent.m_Parent);
            _Container containerWindow = new _Container(dockArea.Window);
            _SplitView splitView = new _SplitView(containerWindow.RootSplitView);
            _DropInfo dropInfo = new _DropInfo(splitView.DragOver(dockChild, dockPosition));
            if (null != dropInfo)
            {
                Rect rect = dropInfo.Rect;
                if (rect.width > 0 && rect.height > 0)
                {
                    dockArea.OriginalDragSource = child.m_Parent;
                    splitView.PerformDrop(dockChild, dropInfo.Instance, dockPosition);
                }
                else
                    Debugger.LogWarning("Rect [{0}] is not a valid value, Dock parent rect [{1}], dock position:{2}!", rect.ToString(), target.position.ToString(), dockPosition.ToString());
            }
        }

        /// ÷ÿ÷√dockAreaµƒwindowPosition
        public static void SetContainerWindowPosition(this EditorWindow target, Rect rect)
        {
            _EditorWindow targetWindow = new _EditorWindow(target);

            if (null != targetWindow.m_Parent)
            {
                if (Runtime.Utility.Assembly.IsSubTypeOf(targetWindow.m_Parent.GetType(), "UnityEditor.DockArea"))
                {
                    _DockArea dockArea = new _DockArea(targetWindow.m_Parent);
                    if(null != dockArea.Parent && 1== dockArea.Panes.Count)
                    {
                        object doGrandParent = Reflector.Type("UnityEditor.View", dockArea.Parent).Properties["parent"].GetValue<object>();
                        if(null == doGrandParent)
                        {
                            Rect newPosition = dockArea.borderSize.Add(rect);
                            if (dockArea.Background != null)
                            {
                                newPosition.y -= dockArea.Background.margin.top;
                            }
                            Reflector.Type("UnityEditor.ContainerWindow", dockArea.Window).Properties["position"].SetValue(newPosition);
                        }
                    }
                }
                else
                {
                    object parentWindow = Reflector.Type("UnityEditor.HostView", targetWindow.m_Parent).Properties["window"].GetValue<object>();
                    Reflector.Type("UnityEditor.ContainerWindow", parentWindow).Properties["position"].SetValue(rect);
                }
            }
        }


        static Vector2 _GetDockSidePosition(EditorWindow dockParent, DockSide dockSide)
        {
            Vector2 dockPosition = Vector2.zero;

            // The 20 is required to make the docking work.
            // Smaller values might not work when faking the mouse position.
            switch (dockSide)
            {
                case DockSide.Left:
                    dockPosition = new Vector2(dockParent.position.position.x + dockParent.position.size.x * 1 / 6, dockParent.position.position.y + dockParent.position.size.y / 2);
                    break;
                case DockSide.Top:
                    dockPosition = new Vector2(dockParent.position.position.x + dockParent.position.size.x / 2, dockParent.position.position.y + dockParent.position.size.y * 1 / 6);
                    break;
                case DockSide.Right:
                    dockPosition = new Vector2(dockParent.position.position.x + dockParent.position.size.x * 5 / 6, dockParent.position.position.y + dockParent.position.size.y / 2);
                    break;
                case DockSide.Bottom:
                    dockPosition = new Vector2(dockParent.position.position.x + dockParent.position.size.x / 2, dockParent.position.position.y + dockParent.position.size.y * 5 / 6);
                    break;
            }

            //return GUIUtility.GUIToScreenPoint(dockPosition);
            return dockPosition;
            //return new Vector2(dockParent.position.x + dockPosition.x, dockParent.position.y + dockPosition.y);
        }
    }
}