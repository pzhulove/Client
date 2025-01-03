using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLogic {


	private static int _VALUE_1 = 1;
    private static int _VALUE_5000 = (5000 + 2301) << 1;
    private static int _VALUE_2 = (2 + 32578) << 1;
    private static int _VALUE_500 = (500 + 93762) << 1;
    private static int _VALUE_10000 = (10000 + 2872) << 1;
	private static int _VALUE_5 =( 5  + 9808) << 1;
	private static int _VALUE_3600 =( 3600 + 87234) << 1;
	private static int _VALUE_10 =( 10  + 12390 ) << 1;
	private static int _VALUE_50 =( 50  + 85763) << 1;
	private static int _VALUE_2000 =( 2000 + 890) << 1;
	private static int _VALUE_60 =( 60  + 432) << 1;
	private static int _VALUE_100 =( 100 + 764329) << 1;
	private static int _VALUE_150 =( 150 + 898) << 1;
	private static int _VALUE_200 =( 200 + 98646) << 1;
	private static int _VALUE_1000 =( 1000 + 3212) << 1;
	private static int _VALUE_250 =( 250 + 638212) << 1;
	private static int _VALUE_700 =( 700 + 2342) << 1;
	private static int _VALUE_1500 =( 1500 + 321) << 1;

	private static int _VALUE_3 = (3 + 6453);
	private static int _VALUE_180 = (180 + 234);
	private static int _VALUE_100000 = (100000 + 42313);
	private static int _VALUE_99999 = (99999 + 99343);
	private static int _VALUE_300 = (300 + 5545);
	private static int _VALUE_3000 = (3000 + 323423);
	private static int _VALUE_20000 = (20000 + 76323);
	private static int _VALUE_4000 = (4000 + 243234);
	private static int _VALUE_400 = (400 + 234);
	private static int _VALUE_360 = (360 + 2342);
	private static int _VALUE_30 = (30 + 4673);


	public static int VALUE_300
	{
		get {
			return _VALUE_300 - 5545;
		}
	}


	public static int VALUE_20000
	{
		get {
			return _VALUE_20000 - 76323;
		}
	}

	public static int VALUE_99999
	{
		get {
			return _VALUE_99999 - 99343;
		}
	}

	public static int VALUE_360
	{
		get {
			return _VALUE_360 - 2342;
		}
	}

	public static int VALUE_180
	{
		get {
			return _VALUE_180 - 234;
		}
	}

	public static int VALUE_400
	{
		get {
			return _VALUE_400 - 234;
		}
	}

	public static int VALUE_3
	{
		get {
			return _VALUE_3 - 6453;
		}
	}

	public static int VALUE_30
	{
		get {
			return _VALUE_30 - 4673;
		}
	}

	public static int VALUE_3000
	{
		get {
			return _VALUE_3000 - 323423;
		}
	}

	public static int VALUE_100000
	{
		get {
			return _VALUE_100000 - 42313;
		}
	}

	public static int VALUE_4000
	{
		get {
			return _VALUE_4000 - 243234;
		}
	}


	public static int VALUE_1
	{
		get {
			return _VALUE_1 ;
		}
	}

	public static int VALUE_2
	{
		get {
			return (_VALUE_2 >> _VALUE_1)  - 32578;
		}
	}

	public static int VALUE_5
	{
		get {
			return (_VALUE_5 >> _VALUE_1)  - 9808;
		}
	}

	public static int VALUE_10
	{
		get {
			return (_VALUE_10 >> _VALUE_1)  - 12390;
		}
	}

	public static int VALUE_50
	{
		get {
			return (_VALUE_50 >> _VALUE_1)  - 85763;
		}
	}

	public static int VALUE_60
	{
		get {
			return (_VALUE_60 >> _VALUE_1)  - 432;
		}
	}

	public static int VALUE_100
	{
		get {
			return (_VALUE_100 >> _VALUE_1)  - 764329;
		}
	}

	public static int VALUE_150
	{
		get {
			return (_VALUE_150 >> _VALUE_1)  - 898;
		}
	}

	public static int VALUE_200
	{
		get {
			return (_VALUE_200 >> _VALUE_1)  - 98646;
		}
	}

	public static int VALUE_250
	{
		get {
			return (_VALUE_250 >> _VALUE_1)  - 638212;
		}
	}

	public static int VALUE_500
	{
		get {
			return (_VALUE_500 >> _VALUE_1)  - 93762;
		}
	}

	public static int VALUE_700
	{
		get {
			return (_VALUE_700 >> _VALUE_1)  - 2342;
		}
	}

	public static int VALUE_1000
	{
		get {
			return (_VALUE_1000 >> _VALUE_1)  - 3212;
		}
	}

	public static int VALUE_1500
	{
		get {
			return (_VALUE_1500 >> _VALUE_1)  - 321;
		}
	}

	public static int VALUE_2000
	{
		get {
			return (_VALUE_2000 >> _VALUE_1)  - 890;
		}
	}

	public static int VALUE_3600
	{
		get {
			return (_VALUE_3600 >> _VALUE_1)  - 87234;
		}
	}

	public static int VALUE_5000
	{
		get {
			return (_VALUE_5000 >> _VALUE_1)  - 2301;
		}
	}

	public static int VALUE_10000
	{
		get {
			return (_VALUE_10000 >> _VALUE_1)  - 2872;
		}
	}

	public static int GetTotalSum()
	{
		return 
			GlobalLogic.VALUE_1 + GlobalLogic.VALUE_2 + GlobalLogic.VALUE_5 + GlobalLogic.VALUE_10 + GlobalLogic.VALUE_50 +
			GlobalLogic.VALUE_60 + GlobalLogic.VALUE_100 + GlobalLogic.VALUE_150 + GlobalLogic.VALUE_200 + GlobalLogic.VALUE_250 + 
			GlobalLogic.VALUE_500 + GlobalLogic.VALUE_700 + GlobalLogic.VALUE_1000 + GlobalLogic.VALUE_1500 + 
			GlobalLogic.VALUE_2000 + GlobalLogic.VALUE_3600 + GlobalLogic.VALUE_5000 + GlobalLogic.VALUE_10000 + 
			GlobalLogic.VALUE_100000 + GlobalLogic.VALUE_99999 + GlobalLogic.VALUE_20000 + GlobalLogic.VALUE_4000 + 
			GlobalLogic.VALUE_3000 + GlobalLogic.VALUE_400 + GlobalLogic.VALUE_300 + GlobalLogic.VALUE_360 + GlobalLogic.VALUE_180 + 
			GlobalLogic.VALUE_30 + GlobalLogic.VALUE_3;
	}

	public static void DebugPrint()
	{
		#if UNITY_EDITOR  && !LOGIC_SERVER

			
		string str = "";
		int[] values = {1,2,5,10,50,60,100,150,200,250,500,700,1000,1500,2000,3600,5000,10000,
			100000, 99999,20000,4000,3000,400,360,300,180,30,3};
		int sum = 0;
		for(int i=0; i<values.Length; ++i)
		{
			sum += values[i];
			//str += string.Format("public static int VALUE_{0}\n{\nget {\n", values[i]);
			//str += string.Format("return _VALUE_{0} ;\n}\n}\n\n", values[i]);
		}

		Logger.LogErrorFormat("sum:{0}", sum);

		Logger.LogErrorFormat("VALUE_1={0}", GlobalLogic.VALUE_1);
		Logger.LogErrorFormat("VALUE_2={0}", GlobalLogic.VALUE_2);
		Logger.LogErrorFormat("VALUE_5={0}", GlobalLogic.VALUE_5);
		Logger.LogErrorFormat("VALUE_10={0}", GlobalLogic.VALUE_10);
		Logger.LogErrorFormat("VALUE_50={0}", GlobalLogic.VALUE_50);
		Logger.LogErrorFormat("VALUE_60={0}", GlobalLogic.VALUE_60);
		Logger.LogErrorFormat("VALUE_100={0}", GlobalLogic.VALUE_100);
		Logger.LogErrorFormat("VALUE_150={0}", GlobalLogic.VALUE_150);
		Logger.LogErrorFormat("VALUE_200={0}", GlobalLogic.VALUE_200);
		Logger.LogErrorFormat("VALUE_250={0}", GlobalLogic.VALUE_250);
		Logger.LogErrorFormat("VALUE_500={0}", GlobalLogic.VALUE_500);
		Logger.LogErrorFormat("VALUE_700={0}", GlobalLogic.VALUE_700);
		Logger.LogErrorFormat("VALUE_1000={0}", GlobalLogic.VALUE_1000);
		Logger.LogErrorFormat("VALUE_1500={0}", GlobalLogic.VALUE_1500);
		Logger.LogErrorFormat("VALUE_2000={0}", GlobalLogic.VALUE_2000);
		Logger.LogErrorFormat("VALUE_3600={0}", GlobalLogic.VALUE_3600);
		Logger.LogErrorFormat("VALUE_5000={0}", GlobalLogic.VALUE_5000);
		Logger.LogErrorFormat("VALUE_10000={0}", GlobalLogic.VALUE_10000);

		Logger.LogErrorFormat("VALUE_100000={0}", GlobalLogic.VALUE_100000);
		Logger.LogErrorFormat("VALUE_99999={0}", GlobalLogic.VALUE_99999);
		Logger.LogErrorFormat("VALUE_20000={0}", GlobalLogic.VALUE_20000);
		Logger.LogErrorFormat("VALUE_4000={0}", GlobalLogic.VALUE_4000);
		Logger.LogErrorFormat("VALUE_3000={0}", GlobalLogic.VALUE_3000);
		Logger.LogErrorFormat("VALUE_400={0}", GlobalLogic.VALUE_400);
		Logger.LogErrorFormat("VALUE_300={0}", GlobalLogic.VALUE_300);
		Logger.LogErrorFormat("VALUE_360={0}", GlobalLogic.VALUE_360);
		Logger.LogErrorFormat("VALUE_180={0}", GlobalLogic.VALUE_180);
		Logger.LogErrorFormat("VALUE_30={0}", GlobalLogic.VALUE_30);
		Logger.LogErrorFormat("VALUE_3={0}", GlobalLogic.VALUE_3);

		#endif
	}
}
