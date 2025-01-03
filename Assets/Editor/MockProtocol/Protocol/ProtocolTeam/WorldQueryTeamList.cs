using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  查询队伍列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 查询队伍列表", " 查询队伍列表")]
	public class WorldQueryTeamList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601623;
		public UInt32 Sequence;
		/// <summary>
		///  根据队伍编号搜索
		/// </summary>
		[AdvancedInspector.Descriptor(" 根据队伍编号搜索", " 根据队伍编号搜索")]
		public UInt16 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public UInt32 targetId;
		/// <summary>
		///  根据地下城搜索
		/// </summary>
		[AdvancedInspector.Descriptor(" 根据地下城搜索", " 根据地下城搜索")]
		public UInt32[] targetList = new UInt32[0];
		/// <summary>
		///  查询起始位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询起始位置", " 查询起始位置")]
		public UInt16 startPos;
		/// <summary>
		///  请求个数
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求个数", " 请求个数")]
		public byte num;
		/// <summary>
		///  参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数", " 参数")]
		public byte param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, targetId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetList.Length);
			for(int i = 0; i < targetList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, targetList[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, startPos);
			BaseDLL.encode_int8(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
			UInt16 targetListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref targetListCnt);
			targetList = new UInt32[targetListCnt];
			for(int i = 0; i < targetList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetList[i]);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref startPos);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref param);
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
