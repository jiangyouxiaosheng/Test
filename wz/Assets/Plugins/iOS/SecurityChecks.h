// SecurityChecks.h
#import <Foundation/Foundation.h>

@interface SecurityChecks : NSObject

+ (BOOL)isVpn;
+ (BOOL)isSimul;
+ (BOOL)sysVNMatch;
+ (BOOL)isDJbPath;
+ (BOOL)isDJbUSche;
+ (BOOL)isDJbLib;
+ (BOOL)idTamper:(NSString *)apiBundleIdentifier;
+ (BOOL)injectLib;

@end