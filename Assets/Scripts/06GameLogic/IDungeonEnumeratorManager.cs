using UnityEngine;
using System.Collections;

public interface IDungeonEnumeratorManager
{
    /// <summary>
    /// 添加一个csharp原声迭代器iter
    /// </summary>
    IEnumerator AddEnumerator(IEnumerator iter, int priority = int.MaxValue);

    void RemoveEnumerator(IEnumerator iter);

    void ClearAllEnumerators();
}
