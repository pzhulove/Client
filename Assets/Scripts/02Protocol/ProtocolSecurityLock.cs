using System;
using System.Text;

namespace Protocol
{
	public enum LockOpType
	{
		LT_LOCK = 1,
		/// <summary>
		///  上锁
		/// </summary>
		LT_UNLOCK = 2,
		/// <summary>
		///  解锁
		/// </summary>
		LT_FORCE_UNLOCK = 3,
		/// <summary>
		///  强制解锁
		/// </summary>
		LT_CANCAL_APPLY = 4,
	}

	/// <summary>
	///  取消申请
	/// </summary>
	public enum SecurityLockState
	{
		SECURITY_STATE_UNLOCK = 0,
		/// <summary>
		///  没锁
		/// </summary>
		SECURITY_STATE_LOCK = 1,
		/// <summary>
		///  锁住
		/// </summary>
		SECURITY_STATE_APPLY = 2,
	}

	/// <summary>
	///  申请中(强制解锁)
	/// </summary>
	/// <summary>
	///  请求安全锁信息(登录发送该消息)
	/// </summary>
	[Protocol]
	public class WorldSecurityLockDataReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608402;
		public UInt32 Sequence;
		/// <summary>
		///  设备ID，客户端没有id时，发送空字符
		/// </summary>
		public string deviceID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// deviceID
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(deviceID);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求安全锁信息返回
	/// </summary>
	[Protocol]
	public class WorldSecurityLockDataRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608403;
		public UInt32 Sequence;
		/// <summary>
		///  锁状态(SecurityLockState)
		/// </summary>
		public UInt32 lockState;
		/// <summary>
		///  是否常用设备
		/// </summary>
		public UInt32 isCommonDev;
		/// <summary>
		///  是否上过锁
		/// </summary>
		public UInt32 isUseLock;
		/// <summary>
		///  冻结时间
		/// </summary>
		public UInt32 freezeTime;
		/// <summary>
		///  解冻时间
		/// </summary>
		public UInt32 unFreezeTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lockState);
				BaseDLL.encode_uint32(buffer, ref pos_, isCommonDev);
				BaseDLL.encode_uint32(buffer, ref pos_, isUseLock);
				BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
				BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isCommonDev);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isUseLock);
				BaseDLL.decode_uint32(buffer, ref pos_, ref freezeTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unFreezeTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lockState);
				BaseDLL.encode_uint32(buffer, ref pos_, isCommonDev);
				BaseDLL.encode_uint32(buffer, ref pos_, isUseLock);
				BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
				BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isCommonDev);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isUseLock);
				BaseDLL.decode_uint32(buffer, ref pos_, ref freezeTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unFreezeTime);
			}

			public int getLen()
			{
				int _len = 0;
				// lockState
				_len += 4;
				// isCommonDev
				_len += 4;
				// isUseLock
				_len += 4;
				// freezeTime
				_len += 4;
				// unFreezeTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// *******************************************
	/// </summary>
	/// <summary>
	///  请求操作安全锁
	/// </summary>
	[Protocol]
	public class WorldSecurityLockOpReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608404;
		public UInt32 Sequence;
		/// <summary>
		///  锁操作类型(LockOpType)
		/// </summary>
		public UInt32 lockOpType;
		/// <summary>
		///  密码
		/// </summary>
		public string passwd;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// lockOpType
				_len += 4;
				// passwd
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(passwd);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求操作安全锁返回
	/// </summary>
	[Protocol]
	public class WorldSecurityLockOpRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608405;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;
		/// <summary>
		///  锁操作类型(LockOpType)
		/// </summary>
		public UInt32 lockOpType;
		/// <summary>
		///  锁状态(SecurityLockState)
		/// </summary>
		public UInt32 lockState;
		/// <summary>
		///  冻结时间
		/// </summary>
		public UInt32 freezeTime;
		/// <summary>
		///  解冻时间
		/// </summary>
		public UInt32 unFreezeTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
				BaseDLL.encode_uint32(buffer, ref pos_, lockState);
				BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
				BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref freezeTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unFreezeTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
				BaseDLL.encode_uint32(buffer, ref pos_, lockState);
				BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
				BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref freezeTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unFreezeTime);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				// lockOpType
				_len += 4;
				// lockState
				_len += 4;
				// freezeTime
				_len += 4;
				// unFreezeTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// ***********************************************
	/// </summary>
	/// <summary>
	///  请求修改安全锁密码
	/// </summary>
	[Protocol]
	public class WorldChangeSecurityPasswdReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608406;
		public UInt32 Sequence;
		/// <summary>
		///  旧密码
		/// </summary>
		public string oldPasswd;
		/// <summary>
		///  新密码
		/// </summary>
		public string newPasswd;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] oldPasswdBytes = StringHelper.StringToUTF8Bytes(oldPasswd);
				BaseDLL.encode_string(buffer, ref pos_, oldPasswdBytes, (UInt16)(buffer.Length - pos_));
				byte[] newPasswdBytes = StringHelper.StringToUTF8Bytes(newPasswd);
				BaseDLL.encode_string(buffer, ref pos_, newPasswdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 oldPasswdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref oldPasswdLen);
				byte[] oldPasswdBytes = new byte[oldPasswdLen];
				for(int i = 0; i < oldPasswdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref oldPasswdBytes[i]);
				}
				oldPasswd = StringHelper.BytesToString(oldPasswdBytes);
				UInt16 newPasswdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newPasswdLen);
				byte[] newPasswdBytes = new byte[newPasswdLen];
				for(int i = 0; i < newPasswdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newPasswdBytes[i]);
				}
				newPasswd = StringHelper.BytesToString(newPasswdBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] oldPasswdBytes = StringHelper.StringToUTF8Bytes(oldPasswd);
				BaseDLL.encode_string(buffer, ref pos_, oldPasswdBytes, (UInt16)(buffer.Length - pos_));
				byte[] newPasswdBytes = StringHelper.StringToUTF8Bytes(newPasswd);
				BaseDLL.encode_string(buffer, ref pos_, newPasswdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 oldPasswdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref oldPasswdLen);
				byte[] oldPasswdBytes = new byte[oldPasswdLen];
				for(int i = 0; i < oldPasswdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref oldPasswdBytes[i]);
				}
				oldPasswd = StringHelper.BytesToString(oldPasswdBytes);
				UInt16 newPasswdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newPasswdLen);
				byte[] newPasswdBytes = new byte[newPasswdLen];
				for(int i = 0; i < newPasswdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newPasswdBytes[i]);
				}
				newPasswd = StringHelper.BytesToString(newPasswdBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// oldPasswd
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(oldPasswd);
					_len += 2 + _strBytes.Length;
				}
				// newPasswd
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(newPasswd);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求修改安全锁密码返回
	/// </summary>
	[Protocol]
	public class WorldChangeSecurityPasswdRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608407;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;
		/// <summary>
		///  错误次数
		/// </summary>
		public UInt32 errNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, errNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, errNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errNum);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				// errNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ***********************************************
	/// </summary>
	/// <summary>
	///  请求绑定或解绑设备
	/// </summary>
	[Protocol]
	public class WorldBindDeviceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608408;
		public UInt32 Sequence;
		/// <summary>
		///  绑定类型，0解绑，否则绑定
		/// </summary>
		public UInt32 bindType;
		/// <summary>
		///  设备ID，客户端没有id时，发送空字符
		/// </summary>
		public string deviceID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, bindType);
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref bindType);
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, bindType);
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref bindType);
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// bindType
				_len += 4;
				// deviceID
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(deviceID);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求绑定或解绑设备返回
	/// </summary>
	[Protocol]
	public class WorldBindDeviceRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608409;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;
		/// <summary>
		///  绑定状态，0没绑，否则绑定
		/// </summary>
		public UInt32 bindState;
		/// <summary>
		///  设备ID
		/// </summary>
		public string deviceID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, bindState);
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bindState);
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, bindState);
				byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
				BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bindState);
				UInt16 deviceIDLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
				byte[] deviceIDBytes = new byte[deviceIDLen];
				for(int i = 0; i < deviceIDLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
				}
				deviceID = StringHelper.BytesToString(deviceIDBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				// bindState
				_len += 4;
				// deviceID
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(deviceID);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// *******************************************
	/// </summary>
	/// <summary>
	///  安全锁禁止操作
	/// </summary>
	[Protocol]
	public class WorldSecurityLockForbidNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608410;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  安全锁密码错误次数
	/// </summary>
	[Protocol]
	public class WorldSecurityLockPasswdErrorNum : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608411;
		public UInt32 Sequence;
		public UInt32 error_num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, error_num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref error_num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, error_num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref error_num);
			}

			public int getLen()
			{
				int _len = 0;
				// error_num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// **********************************************
	/// </summary>
	/// <summary>
	///  网关解锁安全锁
	/// </summary>
	[Protocol]
	public class GateSecurityLockRemoveReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 308401;
		public UInt32 Sequence;
		/// <summary>
		///  密码
		/// </summary>
		public string passwd;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// passwd
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(passwd);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  网关解锁安全锁返回
	/// </summary>
	[Protocol]
	public class GateSecurityLockRemoveRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 308402;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

}
