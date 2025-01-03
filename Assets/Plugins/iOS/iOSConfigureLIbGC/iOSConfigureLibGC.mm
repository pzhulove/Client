
extern "C"
{
    typedef unsigned long GC_word;
    extern GC_word GC_free_space_divisor;
    
    void bfutils_SetLibGCFreeSpaceDivisor(int divisor)
    {
        GC_free_space_divisor = divisor;
    }
}
