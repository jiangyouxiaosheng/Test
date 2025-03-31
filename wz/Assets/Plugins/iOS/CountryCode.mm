#import "CountryCode.h"
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>


@implementation CountryCode

+ (const char *)getCountryCode {
    
    CTTelephonyNetworkInfo *networkInfo = [[CTTelephonyNetworkInfo alloc] init];
    NSDictionary *carriers = networkInfo.serviceSubscriberCellularProviders;
    
    for (CTCarrier *carrier in carriers.allValues) {
        NSString *mcc = carrier.mobileCountryCode;
        if (mcc) {
            const char*utf8String = [mcc UTF8String];
            char *copiedString = strdup(utf8String);
            return copiedString;
        }
    }
    
    return nil;
}

@end

extern "C" {
    const char *GetCountryCode() {
        return [CountryCode getCountryCode];
    }
}
