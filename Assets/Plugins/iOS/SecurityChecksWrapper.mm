// SecurityChecksWrapper.mm
#import <SecurityChecks.h>

extern "C" {

    // 导出为 C 函数供 Unity 调用
    bool IsVpn() {
        return [SecurityChecks isVpn];
    }

    bool IsSimul() {
        return [SecurityChecks isSimul];
    }
    
    bool SysVNMatch() {
        return [SecurityChecks sysVNMatch];
    }
    bool IsDJbPath() {
        return [SecurityChecks isDJbPath];
    }
    bool IsDJbUSche() {
        return [SecurityChecks isDJbUSche];
    }
    bool IsDJbLib() {
        return [SecurityChecks isDJbLib];
    }

    bool IdTamper(const char *apiBundleIdentifier) {
        NSString *identifier = [NSString stringWithUTF8String:apiBundleIdentifier];
        return [SecurityChecks idTamper:identifier];
    }
    bool InjectLib() {
        return [SecurityChecks injectLib];
    }
}