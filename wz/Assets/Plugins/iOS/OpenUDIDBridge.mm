#import "OpenUDIDPlugin.h"

extern "C" {
    const char * GetOpenUDID() {
        NSString *udid = [OpenUDIDPlugin getOpenUDID];
        const char *utf8String = [udid UTF8String];
        char *copiedString = strdup(utf8String); // 复制字符串
        return copiedString;
    }
}