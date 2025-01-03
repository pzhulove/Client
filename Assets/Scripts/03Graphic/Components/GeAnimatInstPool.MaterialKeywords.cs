using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GeAnimatInstPool : Singleton<GeAnimatInstPool>, IObjectPool
{
    /// <summary>
    /// 由一组keywords得到一个唯一的id
    /// </summary>
    public class MaterialKeyWords
    {
        private Dictionary<string, int> m_Keywords = new Dictionary<string, int>();
        private int m_currentBit = 0;

        private int AddKeyword(string keyword)
        {
            int id = 0;
            if (!m_Keywords.TryGetValue(keyword, out id))
            {
                id = m_currentBit;
                m_Keywords.Add(keyword, m_currentBit);
                m_currentBit++;
#if UNITY_EDITOR
                if (m_currentBit > 31)
                {
                    throw new System.Exception("MaterialKeyWords bit out of range");
                }
#endif
            }

            return id;
        }

        public int GetKeywordsUniqueID(IEnumerable<string> keywords)
        {
            int uniqueID = 0;
            foreach(string keyword in keywords)
            {
                uniqueID |= (1 << AddKeyword(keyword));
            }

            return uniqueID;
        }


        public int GetKeywordsUniqueID(string keyword)
        {
            return (1 << AddKeyword(keyword));
        }
    }
}
