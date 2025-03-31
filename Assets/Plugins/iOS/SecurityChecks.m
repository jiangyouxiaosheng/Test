// SecurityChecks.m
#import "SecurityChecks.h"
#import <UIKit/UIKit.h>
#import <objc/runtime.h>
#import <dlfcn.h>
#import <mach-o/dyld.h>
#import <objc/runtime.h>
@implementation SecurityChecks

// 1. VPN 检测
+ (BOOL)isVpn {
    NSDictionary *settings = (__bridge NSDictionary *)CFNetworkCopySystemProxySettings();
    return [settings[@"HTTPProxy"] length] > 0 || [settings[@"HTTPSProxy"] length] > 0;
}

// 2. 模拟器检测
+ (BOOL)isSimul {
    #if TARGET_OS_SIMULATOR
        return YES;
    #else
        return [NSProcessInfo.processInfo.environment[@"SIMULATOR_DEVICE_NAME"] length] > 0;
    #endif
}

// SecurityChecks.m 中的 sysVNMatch 方法
+ (BOOL)sysVNMatch {
    // 获取当前系统主版本号（如 iOS 14.2 的主版本号为 14）
    NSOperatingSystemVersion osVersion = [NSProcessInfo processInfo].operatingSystemVersion;
    NSInteger majorVersion = osVersion.majorVersion;

    // 定义要检测的类及其最低支持的系统主版本
    NSDictionary *apiChecks = @{
        @"PDFView": @11,         // iOS 11.0+
        @"UITextInputPasswordRules": @12,  // iOS 12.0+
        @"UINavigationBarAppearance": @13, // iOS 13.0+
        @"UIWindowSceneGeometryPreferencesIOS": @13, // iOS 13.0+
        @"AVPictureInPictureControllerContentSource": @14, // iOS 14.0+
        @"UIBackgroundConfiguration": @15, // iOS 15.0+
        @"UIContentUnavailableConfiguration": @16  // iOS 16.0+
    };

    // 遍历所有检测项
    for (NSString *className in apiChecks.allKeys) {
        NSInteger minVersion = [apiChecks[className] integerValue];
        
        // 如果当前系统版本 < 最低支持版本，但类存在，则判定为异常
        if (majorVersion < minVersion) {
            Class cls = NSClassFromString(className);
            if (cls != nil) {
                return YES; // 检测到不匹配
            }
        }
    }
    
    return NO; // 未发现异常
}



// SecurityChecks.m 中的 isDJbPath 方法
+ (BOOL)isDJbPath {
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    // 完整路径列表（修正语法错误）
    NSArray *paths = @[
        @"/.cydia_no_stash",
        @"/.installed_unc0ver",
        @"/jb/offsets.plist",
        @"/usr/share/jailbreak/injectme.plist",
        @"/etc/apt/undecimus/undecimus.list",
        @"/var/lib/dpkg/info/mobilesubstrate.md5sums",
        @"/Library/MobileSubstrate/MobileSubstrate.dylib",
        @"/jb/jailbreakd.plist",
        @"/jb/amfid_payload.dylib",
        @"/jb/libjailbreak.dylib",
        @"/usr/sbin/frida-server",
        @"/etc/apt/sources.list.d/electra.list",
        @"/etc/apt/sources.list.d/sileo.sources",
        @"/.bootstrapped_electra",
        @"/usr/lib/libjailbreak.dylib",
        @"/jb/lzma",
        @"/Applications/Icy.app",
        @"/Applications/MxTube.app",
        @"/Applications/RockApp.app",
        @"/Applications/blackra1n.app",
        @"/Applications/SBSettings.app",
        @"/Applications/FakeCarrier.app",
        @"/Applications/WinterBoard.app",
        @"/Applications/IntelliScreen.app",
        @"/var/mobile/Library/Preferences/ABPattern",
        @"/usr/lib/ABDYLD.dylib",
        @"/usr/lib/ABSubLoader.dylib",
        @"/usr/libexec/cydia/firmware.sh",
        @"/var/lib/cydia",
        @"/etc/apt",
        @"/private/var/lib/apt",
        @"/private/var/Users/",
        @"/var/log/apt",
        @"/Applications/Cydia.app",
        @"/private/var/stash",
        @"/private/var/apt",
        @"/private/var/ssh",
        @"/private/var/master.passwd",
        @"/private/var/zlogin",
        @"/private/var/sudo_logsrvd.conf",
        @"/private/var/suid_profile",
        @"/private/var/zlogin",
        @"/private/var/zlogout",
        @"/private/var/zprofile",
        @"/private/var/zshenv",
        @"/private/var/zshrc",
        @"/Applications/iFile.app",
        @"/private/var/lib/apt/",
        @"/private/var/lib/cydia",
        @"/private/var/cache/apt/",
        @"/private/var/log/syslog",
        @"/private/var/tmp/cydia.log",
        @"/var/root/Library/Preferences/com.xina.jailbreak.plist",
        @"/var/root/Library/Preferences/com.xina.blacklist.plist",
        @"/var/mobile/Library/Preferences/com.xina.jailbreak.plist",
        @"/var/mobile/Library/SplashBoard/Snapshots/com.xina.jailbreak",
        @"/Applications/Filza.app",
        @"/var/mobile/Library/Application Support/Containers/com.tigisoftware.Filza",
        @"/var/mobile/Library/Saved Application State/com.tigisoftware.Filza.savedState",
        @"/var/mobile/Library/HTTPStorages/com.tigisoftware.Filza",
        @"/var/mobile/Library/SplashBoard/Snapshots/com.tigisoftware.Filza",
        @"/var/mobile/Library/Filza",
        @"/var/mobile/Library/Preferences/com.tigisoftware.Filza.plist",
        @"/var/mobile/Library/Caches/com.tigisoftware.Filza",
        @"/Applications/Flex3.app",
        @"/var/mobile/Library/Cookies/com.johncoates.Flex.binarycookies",
        @"/var/mobile/Library/Flex3",
        @"/var/mobile/Library/UserConfigurationProfiles/PublicInfo/Flex3Patches.plist",
        @"/Applications/NewTerm.app",
        @"/var/root/.bash_history",
        @"/var/root/Library/Caches/shshd",
        @"/var/root/Library/HTTPStorages/shshd",
        @"/var/mobile/.ekenablelogging",
        @"/var/mobile/.eksafemode",
        @"/Library/MobileSubstrate/DynamicLibraries/SSLKillSwitch2.plist",
        @"/Library/MobileSubstrate/DynamicLibraries/PreferenceLoader.plist",
        @"/Library/MobileSubstrate/DynamicLibraries/PreferenceLoader.dylib",
        @"/Library/MobileSubstrate/DynamicLibraries",
        @"/var/mobile/Library/Preferences/me.jjolano.shadow.plist",
        @"/var/mobile/Library/Saved Application State/ru.domo.cocoatop64.savedState",
        @"/var/mobile/Library/Preferences/ru.domo.cocoatop64.plist",
        @"/var/mobile/Library/SplashBoard/Snapshots/ru.domo.cocoatop64",
        @"/var/root/Library/Preferences/ws.hbang.Terminal.plist",
        @"/var/mobile/Library/Preferences/ws.hbang.Terminal.plist",
        @"/var/mobile/Library/Saved Application State/ws.hbang.Terminal.savedState",
        @"/var/mobile/Library/HTTPStorages/ws.hbang.Terminal",
        @"/var/mobile/Library/SplashBoard/Snapshots/ws.hbang.Terminal",
        @"/var/mobile/Library/Caches/ws.hbang.Terminal",
        @"/var/mobile/Library/Caches/Cephei",
        @"/private/var/mobile/Library/SBSettings/Themes",
        @"/Library/MobileSubstrate/CydiaSubstrate.dylib",
        @"/System/Library/LaunchDaemons/com.ikey.bbot.plist",
        @"/Library/MobileSubstrate/DynamicLibraries/Veency.plist",
        @"/Library/MobileSubstrate/DynamicLibraries/LiveClock.plist",
        @"/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
        @"/var/binpack",
        @"/Library/PreferenceBundles/LibertyPref.bundle",
        @"/Library/PreferenceBundles/ShadowPreferences.bundle",
        @"/Library/PreferenceBundles/ABypassPrefs.bundle",
        @"/Library/PreferenceBundles/FlyJBPrefs.bundle",
        @"/Library/PreferenceBundles/FlyJBPrefs.bundle",
        @"/Library/PreferenceBundles/Cephei.bundle",
        @"/Library/PreferenceBundles/SubstitutePrefs.bundle",
        @"/Library/PreferenceBundles/libhbangprefs.bundle",
        @"/usr/lib/libhooker.dylib",
        @"/usr/lib/libsubstitute.dylib",
        @"/usr/lib/substrate",
        @"/usr/lib/TweakInject",
        @"/var/binpack/Applications/loader.app",
        @"/bin/bash",
        @"/usr/sbin/sshd",
        @"/usr/libexec/ssh-keysign",
        @"/bin/sh",
        @"/etc/ssh/sshd_config",
        @"/usr/libexec/sftp-server",
        @"/usr/bin/ssh",
        @"/usr/sbin/frida-server",
        @"/Applications/FlyJB.app",
        @"/Applications/Zebra.app",
        @"/var/mobile/Library/Application Support/xyz.willy.Zebra",
        @"/var/mobile/Library/Application Support/Containers/xyz.willy.Zebra",
        @"/var/mobile/Library/WebKit/xyz.willy.Zebra",
        @"/var/mobile/Library/Saved Application State/xyz.willy.Zebra.savedState",
        @"/var/mobile/Library/HTTPStorages/xyz.willy.Zebra",
        @"/var/mobile/Library/SplashBoard/Snapshots/xyz.willy.Zebra",
        @"/var/mobile/Library/Caches/xyz.willy.Zebra",
        @"/Library/BawAppie/ABypass",
        @"/Applications/Sileo.app",
        @"/var/mobile/Library/Sileo",
        @"/var/mobile/Library/Preferences/org.coolstar.SileoStore.plist",
        @"/var/mobile/Library/HTTPStorages/org.coolstar.SileoStore",
        @"/var/mobile/Library/Application Support/Containers/org.coolstar.SileoStore",
        @"/var/mobile/Library/Saved Application State/org.coolstar.SileoStore.savedState",
        @"/var/mobile/Library/SplashBoard/Snapshots/org.coolstar.SileoStore",
        @"/var/mobile/Library/Caches/org.coolstar.SileoStore"
    ];

    // 遍历检查路径
    for (NSString *path in paths) {
        if ([fileManager fileExistsAtPath:path]) {
            NSLog(@"检测到越狱路径: %@", path);
            return YES; // 任意路径存在即返回 YES
        }
    }
    
    return NO; // 未检测到越狱路径
}


// SecurityChecks.m 中的 isDJbUSche 方法
+ (BOOL)isDJbUSche {
    NSArray *jailbrokenURLSchemes = @[
        @"cydia://",
        @"sileo://",
        @"zbra://",
        @"apt-repo://",
        @"postbox://",
        @"xina://",
        @"undecimus://",
        @"icleaner://",
        @"ssh://",
        @"santander://",
        @"filza://",
        @"db-lmvo0l08204d0a0://",
        @"boxsdk-810yk37nbrpwaee5907xc4iz8c1ay3my://",
        @"com.googleusercontent.apps.802910049260-0hf6u.v6nsj21itl94v66tphcqnfl172r://",
        @"activator://"
    ];

    for (NSString *scheme in jailbrokenURLSchemes) {
        NSURL *url = [NSURL URLWithString:scheme];
        if (url && [[UIApplication sharedApplication] canOpenURL:url]) {
            NSLog(@"检测到越狱 URL Scheme: %@", scheme);
            return YES;
        }
    }
    
    return NO;
}

+ (BOOL)isDJbLib {
    // 完整的越狱动态库关键词列表（完全保留原始数据）
    NSArray *jailbreakLibraries = @[
        @"systemhook.dylib",
        @"SubstrateLoader.dylib",
        @"SSLKillSwitch2.dylib",
        @"SSLKillSwitch.dylib",
        @"MobileSubstrate.dylib",
        @"TweakInject.dylib",
        @"CydiaSubstrate",
        @"cynject",
        @"CustomWidgetIcons",
        @"PreferenceLoader",
        @"RocketBootstrap",
        @"WeeLoader",
        @"/.file",
        @"libhooker",
        @"SubstrateInserter",
        @"SubstrateBootstrap",
        @"ABypass",
        @"FlyJB",
        @"Substitute",
        @"Cephei",
        @"Electra",
        @"AppSyncUnified-FrontBoard.dylib",
        @"Shadow",
        @"FridaGadget",
        @"frida",
        @"libcycript",
        @"RevealServer"
    ];

    // 获取当前加载的动态库数量
    uint32_t dyldImageCount = _dyld_image_count();

    // 遍历所有动态库
    for (uint32_t i = 0; i < dyldImageCount; i++) {
        // 获取动态库路径（C 字符串）
        const char *imageNameCStr = _dyld_get_image_name(i);
        
        // 转换为 NSString
        NSString *imageName = [[NSString alloc] initWithUTF8String:imageNameCStr];
        if (!imageName) continue;

        // 检查是否包含越狱关键词
        for (NSString *jailbreakLib in jailbreakLibraries) {
            if ([imageName containsString:jailbreakLib]) {
                NSLog(@"检测到越狱动态库: %@", imageName);
                return YES;
            }
        }
    }
    
    return NO;
}

// SecurityChecks.m 中的 idTamper 方法
+ (BOOL)idTamper:(NSString *)apiBundleIdentifier {
    // 获取本地应用的 bundleIdentifier
    NSString *localBundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    
    if (localBundleIdentifier) {
        // 1. 完全匹配的情况
        if ([apiBundleIdentifier isEqualToString:localBundleIdentifier]) {
            return NO; // 未篡改
        }
        // 2. 包含本地标识符但长度不同
        else if ([apiBundleIdentifier containsString:localBundleIdentifier] && 
                apiBundleIdentifier.length != localBundleIdentifier.length) {
            NSLog(@"检测到篡改：API标识符包含本地标识符但长度异常");
            return YES; // 判定为篡改
        }
        // 3. 完全不匹配
        else {
            NSLog(@"检测到篡改：API标识符与本地标识符不匹配");
            return YES; // 判定为篡改
        }
    } else {
        // 无法获取本地标识符（异常情况）
        NSLog(@"无法获取本地Bundle Identifier");
        return YES; // 默认判定为篡改
    }
}


// 检查库是否在共享缓存中
+ (BOOL) isLibraryInSharedCache :(NSString *) library {
    const char *libraryName = [[library lastPathComponent] UTF8String];
    return _dyld_shared_cache_contains_path(libraryName);
}

// 检查路径是否有效
+ (BOOL) isValidPath :(NSString *)libraryPath {
    return [libraryPath hasPrefix:@"/System/"] || [libraryPath hasPrefix:@"/Developer/"];
}

// 检查库是否在白名单列表中
+ (BOOL) isInWhiteLibraries :(NSString *) libraryPath {
    // 白名单列表
    NSArray<NSString *> *whiteLibraries = @[
        @"/usr/lib/libRPAC.dylib",
        @"/usr/lib/libobjc-trampolines.dylib",
        @"/usr/lib/libMainThreadChecker.dylib",
        @"/usr/lib/libBacktraceRecording.dylib",
        @"/usr/lib/libViewDebuggerSupport.dylib",
        @"/usr/lib/system/introspection/libdispatch.dylib",
        @"/private/preboot/Cryptexes/OS/usr/lib/libobjc-trampolines.dylib",
        @"/private/preboot/Cryptexes/OS/usr/lib/libglInterpose.dylib",
        @"/WZBladeThrow.app",
        @"/usr/lib",
    ];
    for (NSString *whiteLibrary in whiteLibraries) {
        if ([libraryPath containsString:whiteLibrary]) {
            return YES;
        }
    }
    return NO;
}

+ (BOOL)injectLib {
    for (uint32_t i = 0; i < _dyld_image_count(); i++) {
        const char *name = _dyld_get_image_name(i);
        if (name) {
            NSString *libName = [NSString stringWithUTF8String:name];
            if (![self isLibraryInSharedCache:libName] && ![self isInWhiteLibraries:libName] &&
                ![self isValidPath:libName]) {
                NSLog(@"Application: %@", libName);
                return YES;  // 发现可疑库
            }
        }
    }
    return NO;  // 未发现可疑库
}

@end
