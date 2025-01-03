using System.Collections.Generic;
#if !LOGIC_SERVER
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//命令帧处理器缓存池
public class GBControllerAllocator:Singleton<GBControllerAllocator>
{
    List<GraphicBackController>[] m_GBController = new List<GraphicBackController>[(int)GB_CATALOG.MAX_COUNT];
    public GraphicBackController Allocate(byte catalog)
    {
        if (catalog >= (int)GB_CATALOG.MAX_COUNT) return null;
        GraphicBackController newController = null;
        var poolList = m_GBController[catalog];
        if (poolList == null)
        {
            m_GBController[catalog] = new List<GraphicBackController>();
            poolList = m_GBController[catalog];
        }
        int tailIndex = 0;
        if (poolList.Count > 0)
        {
            tailIndex = poolList.Count - 1;
            newController = poolList[tailIndex];
            poolList.RemoveAt(tailIndex);
        }
        else
        {
            switch ((GB_CATALOG)catalog)
            {
                case GB_CATALOG.ATTACH:
                    newController = new AttachGraphicBack();
                    break;
                case GB_CATALOG.EFFECT:
                    newController = new EffectGraphicBack();
                    break;
                case GB_CATALOG.ENTITY:
                    newController = new GeEntityGraphicBack();
                    break;
                case GB_CATALOG.MATERIAL:
                    newController = new AnimatGraphicBack();
                    break;
            }
        }
        if(newController != null)
        {
            newController.OnCreate();
        }
        return newController;
    }
    public void Free(GraphicBackController controller)
    {
        if (controller == null || controller.CataLog() >= GB_CATALOG.MAX_COUNT) return;
        var poolList = m_GBController[(int)controller.CataLog()];
        if (poolList == null)
        {
            Logger.LogErrorFormat("Free Controller error {0}", controller.CataLog());
            return;
        }
        controller.Recycle();
        controller.OnRecycle();
        poolList.Add(controller);
    }
    public void Clear()
    {
        for (int i = 0; i < m_GBController.Length; i++)
        {
            if (m_GBController[i] == null) continue;
            var curControllerList = m_GBController[i];
            for (int j = 0; j < curControllerList.Count; j++)
            {
                curControllerList[j].Clear();
            }
            m_GBController[i].Clear();
            m_GBController[i] = null;
        }
    }

}
#endif