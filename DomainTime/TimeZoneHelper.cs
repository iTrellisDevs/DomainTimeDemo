using System;

namespace DomainTime
{
	public class TimeZoneHelper
	{
		public const string UsEastern = "us/eastern";
		public const string UsCentral = "us/central";
		public const string UsMountain = "us/mountain";
		public const string UsPacific = "us/pacific";
		public const string UsAlaskan = "us/alaska";
		public const string UsHawaii = "us/hawaii";
		public const string Utc = "utc";

		/// <summary>
		/// Return a DateTimeOffset for an input UTC DateTime instant and ISO8601 timezone name string
		/// </summary>
		/// <param name="timeZone"></param>
		/// <param name="utcInstant"></param>
		/// <returns></returns>
		public static DateTimeOffset GetDateTimeOffset(string timeZone, DateTime utcInstant)
		{
			if (utcInstant.Kind != DateTimeKind.Utc)
			{
				throw new ArgumentException("The input DateTime must be of DateTimeKind Utc.", nameof(utcInstant));
			}

			var windowsTimeZone = GetWindowsTimeZoneNameFromIso8601TimeZoneName(timeZone);
			var dateTimeOffset = new DateTimeOffset(utcInstant);
			var converted = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, windowsTimeZone);
			return converted;
		}

		/// <summary>
		/// Take an ISO8601 timezone name and return the windows equivalent, if possible. Using these references:
		/// https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
		/// http://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c
		/// (this is extremely basic -- for something more heavy duty would probably want to use Nodatime)
		/// </summary>
		/// <param name="timeZone"></param>
		/// <returns></returns>
		private static string GetWindowsTimeZoneNameFromIso8601TimeZoneName(string timeZone)
		{
			switch (timeZone.ToLower())
			{
				case UsEastern:
					return "Eastern Standard Time";
				case UsCentral:
					return "Central Standard Time";
				case UsMountain:
					return "Mountain Standard Time";
				case UsPacific:
					return "Pacific Standard Time";
				case UsAlaskan:
					return "Alaskan Standard Time";
				case UsHawaii:
					return "Hawaiian Standard Time";
				case Utc: // todo: confirm this is the iso name for utc
					return "UTC";
			}

			throw new ArgumentException("The specified time zone is not recognized.");
		}
	}
}
