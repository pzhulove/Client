using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求自己战绩返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求自己战绩返回", "Scene->Client 请求自己战绩返回")]
	public class SceneChampionSelfBattleRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509822;
		public UInt32 Sequence;
		/// <summary>
		/// 胜场数
		/// </summary>
		[AdvancedInspector.Descriptor("胜场数", "胜场数")]
		public UInt32 win;
		/// <summary>
		/// 败场数
		/// </summary>
		[AdvancedInspector.Descriptor("败场数", "败场数")]
		public UInt32 lose;
		/// <summary>
		/// 0不能复活 1可以复活 2已复活
		/// </summary>
		[AdvancedInspector.Descriptor("0不能复活 1可以复活 2已复活", "0不能复活 1可以复活 2已复活")]
		public byte relive;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, win);
			BaseDLL.encode_uint32(buffer, ref pos_, lose);
			BaseDLL.encode_int8(buffer, ref pos_, relive);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref win);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lose);
			BaseDLL.decode_int8(buffer, ref pos_, ref relive);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
