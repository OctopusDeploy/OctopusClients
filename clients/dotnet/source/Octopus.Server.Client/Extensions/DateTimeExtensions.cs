#nullable enable
using System;
using NodaTime;
using TimeZoneConverter;

namespace Octopus.Client.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a Windows or IANA (Tzdb) timezone into a IANA based NodaTime.DateTimeZone
    /// </summary>
    /// <param name="timeZoneId">The timezone Id</param>
    /// <returns>The NodaTime.DateTimeZone</returns>
    /// <exception cref="Exception">Thrown if it can't be converted</exception>
    public static DateTimeZone ToDateTimeZone(this string timeZoneId) => timeZoneId.ToDateTimeZoneOrNull() ??
                                                                         throw new Exception(
                                                                             $"Could not parse {timeZoneId} as a Time Zone");

    /// <summary>
    /// Converts a Windows or IANA (Tzdb) timezone into a IANA based NodaTime.DateTimeZone
    /// </summary>
    /// <param name="timeZoneId">The timezone Id</param>
    /// <returns>The NodaTime.DateTimeZone</returns>
    public static DateTimeZone? ToDateTimeZoneOrNull(this string timeZoneId)
    {
        // Not using TimeZoneInfo.TryConvertWindowsIdToIanaId as it may not work https://github.com/dotnet/runtime/issues/69438
        DateTimeZone? ConvertFromWindows()
            => TZConvert.TryWindowsToIana(timeZoneId, out var ianaId)
                ? DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaId)
                : null;

        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId) ?? ConvertFromWindows();
    }

    /// <summary>
    /// Converts a IANA based NodaTime.DateTimeZone to a Windows Time Zone Id
    /// 
    /// All NodaTime.DateTimeZone should be IANA based as that works consistently across platforms
    /// 
    /// However when converted to an ID, it should be a Windows Time Zone ID as that is
    /// the format chosen for the API (long before .NET on Linux). It is also the format
    /// our Time Zone picker uses.
    /// </summary>
    /// <param name="zone">The IANA based zone to get the windows Id for</param>
    /// <returns>The Windows Time Zone ID</returns>
    /// <exception cref="Exception">Throws if the timezone is already a Windows Id, or it can't be parsed</exception>
    public static string ToWindowsTimeZoneId(this DateTimeZone zone) =>
        // Not using TimeZoneInfo.TryConvertIanaIdToWindowsId as it may not work https://github.com/dotnet/runtime/issues/69438
        TZConvert.TryIanaToWindows(zone.Id, out var tzId)
            ? tzId
            : throw new Exception($"Could not convert '{zone.Id}' to a Windows Time Zone ID");
}