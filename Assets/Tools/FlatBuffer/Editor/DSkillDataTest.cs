using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DSkillDataTest
{

    [Test]
    public void TestSkillFileList()
    {
        var allFiles = Directory.GetFiles("Assets/Resources/Data/SkillData/", "*_FileList.json", SearchOption.AllDirectories);
        foreach (var f in allFiles)
        {
            string n = f.Replace("Assets/Resources/", "");
            n = n.Replace("\\", "/");
            var filename = Path.GetFileName(n);
            n = n.Replace(filename, "");

            _TestOne((n));
        }

    }

    private  void _TestOne(string path)
    {
        UnityEngine.Debug.LogFormat("start test {0}", path);

        var fst = BeUtility.GetSkillFileList(path, BeUtility.ESkillFileNameType.Binary);
        var snd = BeUtility.GetSkillFileList(path, BeUtility.ESkillFileNameType.Dir);
        var third = BeUtility.GetSkillFileList(path, BeUtility.ESkillFileNameType.Json);

        _Sort(fst);
        _Sort(snd);
        _Sort(third);

        _IsEqual(path+", binary<->dir", fst, snd);
        _IsEqual(path+", binary<->json", fst, third);
    }

    private void _Sort(List<SkillFileName> list)
    {
        if (null == list)
        {
            return;
        }

        list.Sort((f, s) => f.fullPath.CompareTo(s.fullPath));
    }

    private void _IsEqual(string tag, IList<SkillFileName> fst, IList<SkillFileName> snd)
    {
        if (fst == snd)
        {
            return;
        }

        if (fst == null || snd == null)
        {
            return;
        }

        Assert.AreEqual(fst.Count, snd.Count, tag + "Length not equal");

        for (int i = 0; i < fst.Count; ++i)
        {
            Assert.AreEqual(fst[i], snd[i], tag + ", index: " + i.ToString() + " not equal");
        }
    }
}
