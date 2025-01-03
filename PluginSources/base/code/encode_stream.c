#include "encode_stream.h"

#include <string.h>

#ifdef _WIN32
#include <WinSock2.h>
#pragma  comment(lib, "ws2_32.lib")
#else
#include <netinet/in.h>
#endif

int encode_int8(char *buf, int* pos, unsigned char val)
{
	if (!buf || !pos)
	{
		return -1;
	}

	*((unsigned char*)(buf + *pos)) = val;
	*pos += 1;

	return 0;
}

int decode_int8(char *buf, int* pos, unsigned char* val)
{
	if (!buf || !pos || !val)
	{
		return -1;
	}

	*val = *((unsigned char*)(buf + *pos));
	*pos += 1;

	return 0;
}

int encode_int16(char *buf, int* pos, unsigned short val)
{
	if (!buf || !pos)
	{
		return -1;
	}

	*((unsigned short*)(buf + *pos)) = htons(val);
	*pos += 2;

	return 0;
}

int decode_int16(char *buf, int* pos, unsigned short* val)
{
	if (!buf || !pos || !val)
	{
		return -1;
	}

	*val = ntohs(*((unsigned short*)(buf + *pos)));
	*pos += 2;

	return 0;
}

int encode_int32(char *buf, int* pos, unsigned long val)
{
	if (!buf || !pos)
	{
		return -1;
	}

	*((unsigned int*)(buf + *pos)) = htonl(val);
	*pos += 4;

	return 0;
}

int decode_int32(char *buf, int* pos, unsigned long* val)
{
	if (!buf || !pos || !val)
	{
		return -1;
	}

	*val = ntohl(*((unsigned int*)(buf + *pos)));
	*pos += 4;

	return 0;
}

int encode_int64(char *buf, int* pos, unsigned long long val)
{
	if (!buf || !pos)
	{
		return -1;
	}

	*((unsigned long long*)(buf + *pos)) = htonll(val);
	*pos += 8;

	return 0;
}

int decode_int64(char *buf, int* pos, unsigned long long* val)
{
	if (!buf || !pos || !val)
	{
		return -1;
	}

	*val = ntohll(*((unsigned long long*)(buf + *pos)));
	*pos += 8;

	return 0;
}

int encode_string(char *buf, int* pos, const char* str, unsigned short max_length)
{
	if (!buf || !pos || !str || max_length < 0)
	{
		return -1;
	}

	// ÕâÀï°üÀ¨'\0'
	unsigned short str_length = (unsigned short)strlen(str) + 1;
	if (str_length > max_length)
	{
		str_length = max_length;
	}
	
	encode_int16(buf, pos, str_length);
	memcpy(buf + *pos, str, str_length);
	buf[*pos + str_length - 1] = '\0';
	
	*pos += str_length;

	return 0;
}

int decode_string(char *buf, int* pos, char* str, unsigned short max_length)
{
	if (!buf || !pos || !str || max_length < 0)
	{
		return -1;
	}

	unsigned short str_length = 0;
	decode_int16(buf, pos, &str_length);
	
	unsigned short copy_length = str_length;
	if (copy_length > max_length)
	{
		copy_length = max_length;
	}

	memcpy(str, buf + *pos, copy_length);
	*pos += str_length;

	return 0;
}

int encode_vec_int8(char *buf, int* pos, unsigned char* vec, unsigned short num)
{
	if (!buf || !pos || !vec || num < 0)
	{
		return -1;
	}

	encode_int16(buf, pos, num);
	memcpy(buf + *pos, vec, num);
	*pos += num;

	return 0;
}

int decode_vec_int8(char *buf, int* pos, unsigned char* vec, unsigned short *num)
{
	if (!buf || !pos || !vec || !num || *num < 0)
	{
		return -1;
	}

	int max_num = *num;
	decode_int16(buf, pos, num);
	
	unsigned short copy_num = *num;
	if (copy_num > max_num)
	{
		copy_num = max_num;
	}

	memcpy(vec, buf + *pos, copy_num);
	*pos += *num;

	return 0;
}

int encode_vec_int16(char *buf, int* pos, unsigned short* vec, unsigned short num)
{
	if (!buf || !pos || !vec || num < 0)
	{
		return -1;
	}

	encode_int16(buf, pos, num);
	
	for (int i = 0; i < num; i++)
	{
		encode_int16(buf, pos, vec[i]);
	}

	return 0;
}

int decode_vec_int16(char *buf, int* pos, unsigned short* vec, unsigned short *num)
{
	if (!buf || !pos || !vec || !num || *num < 0)
	{
		return -1;
	}

	int max_num = *num;
	decode_int16(buf, pos, num);

	unsigned short copy_num = *num;
	if (copy_num > max_num)
	{
		copy_num = max_num;
	}

	for (int i = 0; i < copy_num; i++)
	{
		decode_int16(buf, pos, &vec[i]);
	}

	return 0;
}

int encode_vec_int32(char *buf, int* pos, unsigned long* vec, unsigned short num)
{
	if (!buf || !pos || !vec || num < 0)
	{
		return -1;
	}

	encode_int16(buf, pos, num);

	for (int i = 0; i < num; i++)
	{
		encode_int32(buf, pos, vec[i]);
	}

	return 0;
}

int decode_vec_int32(char *buf, int* pos, unsigned long* vec, unsigned short *num)
{
	if (!buf || !pos || !vec || !num || *num < 0)
	{
		return -1;
	}

	int max_num = *num;
	decode_int16(buf, pos, num);

	unsigned short copy_num = *num;
	if (copy_num > max_num)
	{
		copy_num = max_num;
	}

	for (int i = 0; i < copy_num; i++)
	{
		decode_int32(buf, pos, &vec[i]);
	}

	return 0;
}

int encode_vec_int64(char *buf, int* pos, unsigned long long* vec, unsigned short num)
{
	if (!buf || !pos || !vec || num < 0)
	{
		return -1;
	}

	encode_int16(buf, pos, num);

	for (int i = 0; i < num; i++)
	{
		encode_int64(buf, pos, vec[i]);
	}

	return 0;
}

int decode_vec_int64(char *buf, int* pos, unsigned long long* vec, unsigned short *num)
{
	if (!buf || !pos || !vec || !num || *num < 0)
	{
		return -1;
	}

	int max_num = *num;
	decode_int16(buf, pos, num);

	unsigned short copy_num = *num;
	if (copy_num > max_num)
	{
		copy_num = max_num;
	}

	for (int i = 0; i < copy_num; i++)
	{
		decode_int64(buf, pos, &vec[i]);
	}

	return 0;
}

