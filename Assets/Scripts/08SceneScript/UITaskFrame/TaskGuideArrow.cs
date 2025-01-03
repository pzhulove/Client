using UnityEngine;
using System.Collections;

namespace GameClient
{
    public class TaskGuideArrow
    {
        public enum TaskGuideDir : int
        {
            TGD_LEFT = MovingDirectionType.MDT_LEFT,
            TGD_RIGHT = MovingDirectionType.MDT_RIGHT,
            TGD_TOP = MovingDirectionType.MDT_UP,
            TGD_BOTTOM = MovingDirectionType.MDT_DOWN,
            TGD_COUNT,
            TGD_INVALID = -1,
        }
    }
}