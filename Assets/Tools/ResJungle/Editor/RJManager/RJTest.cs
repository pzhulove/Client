using ResJungle;
using UnityEditor;

public class RJTest
{
    //[MenuItem("RJTest/Test1")]
    public static void Test1()
    {
        RJUtility.Log.SetLevel(ERJLogLevel.W);

        var mng = ResJungleFactory.CreateFromRuntime();

        mng.Load("Assets/Tools/ResJungle/All.asset");

        mng.Save();
    }

    //[MenuItem("RJTest/Test2")]
    public static void Test2()
    {
        string path = "../Share/table/xls/";

        var type = typeof(ProtoTable.DungeonTable);
        var filename = Xls2FBWindow.GetXlsNameBySheetName("DungeonTable");

        XlsxDataUnit u = new XlsxDataUnit(path + filename);

        var data = u.GetRowData(4152000);

        var cell = data["BGMPath"];

        cell.SetCellValue("999");

        u.Write();
    }
}

