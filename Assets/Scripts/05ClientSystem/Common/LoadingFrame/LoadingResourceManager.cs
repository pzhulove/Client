using UnityEngine;
using ProtoTable;
using System;
using System.Collections.Generic;
using System.Collections;
public class LoadingResourceManager
{
    static private List<LoadingResourcesTable> m_CityLoading = new List<LoadingResourcesTable>();

    static public List<LoadingResourcesTable> CityLoading
    {
        get {return m_CityLoading;}
    }
    static private List<LoadingResourcesTable> m_DugeonLoading = new List<LoadingResourcesTable>();
    
    static public List<LoadingResourcesTable> DugeonLoading
    {
        get {return m_DugeonLoading;}
    }

    public static void InitLoadingResource()
    {
        DeinitLoadingResource();

        var dics = TableManager.instance.GetTable<LoadingResourcesTable>();
        foreach(var cur in dics)
        {
            if(cur.Value is LoadingResourcesTable)
            {
                var tab = cur.Value as LoadingResourcesTable;
                if(tab.Type == LoadingResourcesTable.eType.AT_EQUIP)
                {
                      m_CityLoading.Add(tab);  
                }
                else if(tab.Type == LoadingResourcesTable.eType.AT_DEFENCE)
                {
                      DugeonLoading.Add(tab);
                }
            }
        }
    }

    public static void DeinitLoadingResource()
    {
        m_CityLoading.Clear();
        m_DugeonLoading.Clear();
    }
    
    static public string GetRandomCityLoadingRes()
    {
          int count = CityLoading.Count;
          int index = UnityEngine.Random.Range(0, count);
          
          if(index < 0 || index >= count)
          {
              return null;
          }

          string path = CityLoading[index].Resources;
          if(path.Length <= 1)
          {
              return null;
          }

          return path;
    }

    static public string GetRandomDugeonLoadingRes()
    {
          int count = DugeonLoading.Count;
          int index = UnityEngine.Random.Range(0, count);
          
          if(index < 0 || index >= count)
          {
              return null;
          }

          string path = DugeonLoading[index].Resources;
          if(path.Length <= 1)
          {
              return null;
          }

          return path;
    }
}