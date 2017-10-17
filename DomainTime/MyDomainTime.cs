using System;

namespace DomainTime
{
	public class MyDomainTime
	{
		private const string DefaultTimeZone = "US/Pacific";
		private string _timeZone;
		private static Func<DateTime> _funcForDeterminingNow;

		static MyDomainTime()
		{
			// by default use UtcNow for the "now" value, but you can override it
			_funcForDeterminingNow = DefaultNowFunc;
		}

		private static DateTime DefaultNowFunc()
		{
			return DateTime.UtcNow;
		}

		private MyDomainTime()
		{
			// private constructor means you have to use one of the static methods below to instantiate
		}

		/// <summary>
		/// Returns whatever "right now" is. Note that it's not a static method like DateTime.Now
		/// </summary>
		/// <returns></returns>
		public DateTimeOffset Now()
		{
			var now = _funcForDeterminingNow.Invoke();
			return TimeZoneHelper.GetDateTimeOffset(_timeZone, now);
		}

		public bool IsLunchtime()
		{
			var now = Now();
			return now.Hour == 12 && now.Minute == 0;
		}

		public bool IsExpired(DateTime expirationDate)
		{
			/* ignore the time component of the input date and compare against midnight local time */ 
			
			var realExpirationDate = new DateTime(expirationDate.Year, expirationDate.Month, expirationDate.Day, 23, 59, 59, DateTimeKind.Local);
			return Now() > realExpirationDate;
		}

		/// <summary>
		/// Override the method by which "now" is determined, so you can test it
		/// </summary>
		/// <param name="nowFunc"></param>
		public static void SetNow(Func<DateTime> nowFunc)
		{
			_funcForDeterminingNow = nowFunc;
		}

		public static void ResetNow()
		{
			_funcForDeterminingNow = DefaultNowFunc;
		}

		/// <summary>
		/// Sets the context as local time for the default timezone
		/// </summary>
		/// <returns></returns>
		public static MyDomainTime FromLocal()
		{
			// you could use an app.config setting (for example) instead of a hard-coded value
			var timeZone = DefaultTimeZone;
			return FromLocal(timeZone);
		}

		/// <summary>
		/// Sets the context as local time for the specified timezone
		/// </summary>
		/// <param name="timeZone"></param>
		/// <returns></returns>
		public static MyDomainTime FromLocal(string timeZone)
		{
			return new MyDomainTime { _timeZone = timeZone };
		}

		/// <summary>
		/// Sets the context as UTC
		/// </summary>
		/// <returns></returns>
		public static MyDomainTime FromUniversal()
		{
			return new MyDomainTime {_timeZone = "utc"};
		}
	}
}
