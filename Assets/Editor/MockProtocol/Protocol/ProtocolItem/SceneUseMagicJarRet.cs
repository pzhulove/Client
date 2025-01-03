using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 返回码
	/// </summary>
	/// <summary>
	/// 开罐子返回
	/// </summary>
	[AdvancedInspector.Descriptor("开罐子返回", "开罐子返回")]
	public class SceneUseMagicJarRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500943;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 jarID;
		/// <summary>
		/// 罐子ID
		/// </summary>
		[AdvancedInspector.Descriptor("罐子ID", "罐子ID")]
		public OpenJarResult[] getItems = null;
		/// <summary>
		/// 抽到的道具
		/// </summary>
		[AdvancedInspector.Descriptor("抽到的道具", "抽到的道具")]
		public ItemReward baseItem = null;
		/// <summary>
		/// 保底道具
		/// </summary>
		[AdvancedInspector.Descriptor("保底道具", "保底道具")]
		public UInt32 getPointId;
		/// <summary>
		/// 获得积分id
		/// </summary>
		[AdvancedInspector.Descriptor("获得积分id", "获得积分id")]
		public UInt32 getPoint;
		/// <summary>
		/// 获得积分数量
		/// </summary>
		[AdvancedInspector.Descriptor("获得积分数量", "获得积分数量")]
		public UInt32 crit;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, jarID);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
			for(int i = 0; i < getItems.Length; i++)
			{
				getItems[i].encode(buffer, ref pos_);
			}
			baseItem.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, getPointId);
			BaseDLL.encode_uint32(buffer, ref pos_, getPoint);
			BaseDLL.encode_uint32(buffer, ref pos_, crit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarID);
			UInt16 getItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
			getItems = new OpenJarResult[getItemsCnt];
			for(int i = 0; i < getItems.Length; i++)
			{
				getItems[i] = new OpenJarResult();
				getItems[i].decode(buffer, ref pos_);
			}
			baseItem.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getPointId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getPoint);
			BaseDLL.decode_uint32(buffer, ref pos_, ref crit);
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
