namespace LibZip
{
	using System;
	using System.Runtime.InteropServices;
	using System.Reflection;
	using System.Collections;
	using System.Text;
	using System.Security;
	
	
	[StructLayout(LayoutKind.Sequential)]
	public struct zip_stat {
		public Int64    valid;			 	/* which fields have valid values */
		public IntPtr 	name;				/* name of the file */
		public Int64	index;				/* index within archive */
		public Int64	size;				/* size of file (uncompressed) */
		public Int64	comp_size;			/* size of file (compressed) */
		public int 		mtime;				/* modification time */
		public int 		crc;				/* crc of file data */
		public short 	comp_method;		/* compression method used */
		public short 	encryption_method;	/* encryption method used */
		public int 		flags;	
	};
	
	
	public class LibZip
	{
#if UNITY_IOS
		private const string LIBZIP_DLL = "__Internal";
#else
        private const string LIBZIP_DLL = "zip";
#endif

        //struct zip *zip_open(const char *, int, int *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_open(string strZipPath, int iMode, IntPtr ptrDefaultNull);

        //int zip_get_num_files(struct zip *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_get_num_files(IntPtr ptrZip);

        //struct zip_file *zip_fopen(struct zip *, const char *, int);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_fopen(IntPtr ptrZip, string strFileName, int iMode);

        //int zip_stat(struct zip *, const char *, int, struct zip_stat *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_stat(IntPtr ptrZip, string strFileName, int iMode, ref zip_stat refZipState);

        //ssize_t zip_fread(struct zip_file *, void *, size_t);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 zip_fread(IntPtr ptrZipFile, byte[] ptrBuffer, Int64 iBufferSize);

        //ZIP_EXTERN int zip_fclose(struct zip_file *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_fclose(IntPtr ptrZipFile);

        //ZIP_EXTERN int zip_close(struct zip *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_close(IntPtr ptrZip);

        [DllImport(LIBZIP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_get_name(IntPtr ptrZip, Int64 iFile, int iMode);

        // ZIP_EXTERN zip_int64_t zip_get_num_entries(zip_t *, zip_flags_t);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 zip_get_num_entries(IntPtr ptrZip, int flag);

        //ZIP_EXTERN int zip_stat_index(zip_t *, zip_uint64_t, zip_flags_t, zip_stat_t *);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_stat_index(IntPtr ptrZip, Int64 iSize, int iFlag, ref zip_stat refZipState);

        //ZIP_EXTERN zip_file_t *zip_fopen_index(zip_t *, zip_uint64_t, zip_flags_t);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_fopen_index(IntPtr ptrZip, Int64 iIndex, int iFlag);

        //ZIP_EXTERN zip_int64_t zip_file_add(zip_t *, const char *, zip_source_t *, zip_flags_t);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 zip_file_add(IntPtr ptrZip, string name, IntPtr source, int iFlag);

        //ZIP_EXTERN zip_source_t *zip_source_buffer(zip_t *, const void *, zip_uint64_t, int);
        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_source_buffer(IntPtr ptrZip, byte[] data, Int64 len, int freep);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_source_write(IntPtr sourcePtr, byte[] data, Int64 len);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_source_begin_write(IntPtr sourcePtr);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zip_source_file(IntPtr zipPtr, string fname, Int64 start, Int64 len);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_source_commit_write(IntPtr sourcePtr);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zip_source_free(IntPtr sourcePtr);

        [DllImport(LIBZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zip_source_rollback_write(IntPtr sourcePtr);
    }
}
