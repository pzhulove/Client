#include <mach/mach.h>

extern "C"
{
    vm_size_t ios_report_memory(void)
    {
        struct task_basic_info info;
        mach_msg_type_number_t size = sizeof(info);
        kern_return_t kerr = task_info(mach_task_self(),
                                       TASK_BASIC_INFO,
                                       (task_info_t)&info,
                                       &size);
        
        if (KERN_SUCCESS == kerr)
        {
            return info.resident_size;
        }
        else
        {
            return 0;
        }
    }
}

