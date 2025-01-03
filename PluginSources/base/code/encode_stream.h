#ifndef __ENCODE_STREAM_H__
#define __ENCODE_STREAM_H__

#ifdef STREAM_DLL_EXPORTS
#ifdef _WIN32
#define STREAM_API __declspec(dllexport)
#else
#define STREAM_API
#endif
#else
#ifdef _WIN32
#define STREAM_API __declspec(dllimport)
#else
#define STREAM_API
#endif
#endif

#ifdef __cplusplus 
extern "C" {
#endif

STREAM_API int encode_int8(char *buf, int* pos, unsigned char val);
STREAM_API int decode_int8(char *buf, int* pos, unsigned char* val);

STREAM_API int encode_int16(char *buf, int* pos, unsigned short val);
STREAM_API int decode_int16(char *buf, int* pos, unsigned short* val);

STREAM_API int encode_int32(char *buf, int* pos, unsigned long val);
STREAM_API int decode_int32(char *buf, int* pos, unsigned long* val);

STREAM_API int encode_int64(char *buf, int* pos, unsigned long long val);
STREAM_API int decode_int64(char *buf, int* pos, unsigned long long* val);

STREAM_API int encode_string(char *buf, int* pos, const char* str, unsigned short max_length);
STREAM_API int decode_string(char *buf, int* pos, char* str, unsigned short max_length);

STREAM_API int encode_vec_int8(char *buf, int* pos, unsigned char* vec, unsigned short num);
STREAM_API int decode_vec_int8(char *buf, int* pos, unsigned char* vec, unsigned short *num /* 传入数组大小，传出实际大小 */);

STREAM_API int encode_vec_int16(char *buf, int* pos, unsigned short* vec, unsigned short num);
STREAM_API int decode_vec_int16(char *buf, int* pos, unsigned short* vec, unsigned short *num /* 传入数组大小，传出实际大小 */);

STREAM_API int encode_vec_int32(char *buf, int* pos, unsigned long* vec, unsigned short num);
STREAM_API int decode_vec_int32(char *buf, int* pos, unsigned long* vec, unsigned short *num /* 传入数组大小，传出实际大小 */);

STREAM_API int encode_vec_int64(char *buf, int* pos, unsigned long long* vec, unsigned short num);
STREAM_API int decode_vec_int64(char *buf, int* pos, unsigned long long* vec, unsigned short *num /* 传入数组大小，传出实际大小 */);

#ifdef __cplusplus 
}
#endif

#endif