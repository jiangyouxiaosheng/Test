#import <Foundation/Foundation.h>
#import "OpenUDID.h"

@interface OpenUDIDPlugin : NSObject

+ (NSString *)getOpenUDID;

@end

@implementation OpenUDIDPlugin

+ (NSString *)getOpenUDID
{
    return [OpenUDID value];
}

@end