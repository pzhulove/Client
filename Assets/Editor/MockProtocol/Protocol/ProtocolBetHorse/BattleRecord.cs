using System;
using System.Text;

namespace Mock.Protocol
{

	public class BattleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  场次id
		/// </summary>
		[AdvancedInspector.Descriptor(" 场次id", " 场次id")]
		public UInt32 courtId;
		/// <summary>
		///  冠军射手
		/// </summary>
		[AdvancedInspector.Descriptor(" 冠军射手", " 冠军射手")]
		public UInt32 champion;
		/// <summary>
		///  赔率
		/// </summary>
		[AdvancedInspector.Descriptor(" 赔率", " 赔率")]
		public UInt32 odds;
		/// <summary>
		///  最大奖金
		/// </summary>
		[AdvancedInspector.Descriptor(" 最大奖金", " 最大奖金")]
		public UInt32 maxProfit;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, courtId);
			BaseDLL.encode_uint32(buffer, ref pos_, champion);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
			BaseDLL.encode_uint32(buffer, ref pos_, maxProfit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxProfit);
		}


		#endregion

	}

}
