using System;
using System.Threading;
using DomainTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTimeTests
{
	[TestClass]
	public class DomainTimeTests
	{
		[TestMethod]
		public void Now_FromLocal_ChangesNowValueToSpecifiedDateTimeWithLocalOffset()
		{
			MyDomainTime.SetNow(() => new DateTime(2015, 11, 4, 8, 45, 12, DateTimeKind.Utc));
			var now = MyDomainTime.FromLocal("US/Pacific").Now();
			Assert.AreEqual(2015, now.Year);
			Assert.AreEqual(11, now.Month);
			Assert.AreEqual(4, now.Day);
			Assert.AreEqual(0, now.Hour); // should be the local hour, given the UTC -8 for this date
			Assert.AreEqual(45, now.Minute);
			Assert.AreEqual(12, now.Second);
			Assert.AreEqual(new TimeSpan(-8, 0, 0), now.Offset);
			MyDomainTime.ResetNow();
		}

		[TestMethod]
		public void Now_FromUniversal_ChangesNowValueToSpecifiedDateTimeWithNoOffset()
		{
			MyDomainTime.SetNow(() => new DateTime(2015, 11, 4, 8, 45, 12, DateTimeKind.Utc));
			var now = MyDomainTime.FromUniversal().Now();
			Assert.AreEqual(2015, now.Year);
			Assert.AreEqual(11, now.Month);
			Assert.AreEqual(4, now.Day);
			Assert.AreEqual(8, now.Hour);
			Assert.AreEqual(45, now.Minute);
			Assert.AreEqual(12, now.Second);
			Assert.AreEqual(new TimeSpan(0, 0, 0), now.Offset);
			MyDomainTime.ResetNow();
		}

		[TestMethod]
		public void Now_GivenUsPacificTimeZone_UsesCorrectOffset()
		{
			// note this is a date when daylight saving time is NOT in effect
			var nowFunc = new Func<DateTime>(() => new DateTime(2014, 2, 23, 5, 0, 0, DateTimeKind.Utc));
			MyDomainTime.SetNow(nowFunc);

			var localTime = MyDomainTime.FromLocal("US/Pacific").Now();
			Assert.IsTrue(localTime.Offset == TimeSpan.FromHours(-8));
			MyDomainTime.ResetNow();
		}

		[TestMethod]
		public void Now_GivenUsEasternTimeZone_UsesCorrectOffset()
		{
			// note this is a date when daylight saving time is in effect
			var nowFunc = new Func<DateTime>(() => new DateTime(2014, 7, 23, 5, 0, 0, DateTimeKind.Utc));

			MyDomainTime.SetNow(nowFunc);
			var localTime = MyDomainTime.FromLocal("US/Eastern").Now();
			Assert.IsTrue(localTime.Offset == TimeSpan.FromHours(-4));
		}

		[TestMethod]
		public void Now_DefaultNowFunction_ReturnsActualNow()
		{
			MyDomainTime.ResetNow();
			var now = DateTime.UtcNow;
			var testNow = MyDomainTime.FromUniversal().Now();
			Assert.AreEqual(now.Year, testNow.Year);
			Assert.AreEqual(now.Month, testNow.Month);
			Assert.AreEqual(now.Day, testNow.Day);
			Assert.AreEqual(now.Hour, testNow.Hour);
			Assert.AreEqual(now.Minute, testNow.Minute);
		}

		[TestMethod]
		public void IsExpired_GivenNoonAndItExpiresAtMidnight_ReturnsFalse()
		{
			var nowFunc = new Func<DateTime>(() => new DateTime(2014, 7, 23, 19, 0, 0, DateTimeKind.Utc));
			MyDomainTime.SetNow(nowFunc);

			var expiresDate = new DateTime(2014, 7, 24);
			var expired = MyDomainTime.FromLocal(TimeZoneHelper.UsPacific).IsExpired(expiresDate);
			Assert.IsFalse(expired);
			MyDomainTime.ResetNow();
		}

		[TestMethod]
		public void IsExpired_GivenNoonAndItExpiredTodayAtMidnight_ReturnsTrue()
		{
			var nowFunc = new Func<DateTime>(() => new DateTime(2014, 7, 23, 19, 0, 0, DateTimeKind.Utc));
			MyDomainTime.SetNow(nowFunc);

			var expiresDate = new DateTime(2014, 7, 23);
			var expired = MyDomainTime.FromLocal(TimeZoneHelper.UsPacific).IsExpired(expiresDate);
			Assert.IsFalse(expired);
			MyDomainTime.ResetNow();
		}

		[TestMethod]
		[Ignore] // for manual testing
		public void Example1()
		{
			while (true)
			{
				if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 0)
				{
					Console.WriteLine("Time for lunch!");
					SetCalendar("Back in 30 minutes.");
					return;
				}
				Thread.Sleep(1000);
			}
		}

		[TestMethod]
		[Ignore] // for manual testing
		public void Example2()
		{
			while (true)
			{
				var now = GetLocalTime(DateTime.UtcNow, -7);
				if (now.Hour == 12 && now.Minute == 0)
				{
					Console.WriteLine("Time for lunch!");
					SetCalendar("Back in 30 minutes.");
					return;
				}
				Thread.Sleep(1000);
			}
		}

		[TestMethod]
		public void Example3()
		{
			MyDomainTime.SetNow(() => new DateTime(2017, 6, 13, 19, 0, 0, DateTimeKind.Utc));
			while (true)
			{
				var domainTime = MyDomainTime.FromLocal(TimeZoneHelper.UsPacific);
				if (domainTime.IsLunchtime())
				{
					Console.WriteLine("Time for lunch!");
					SetCalendar("Back in 30 minutes.");
					MyDomainTime.ResetNow();
					return;
				}
				Thread.Sleep(1000);
			}
		}

		private void SetCalendar(string message)
		{
			// pretend this is more interesting than it is
			Console.WriteLine(message);
		}

		private DateTime GetLocalTime(DateTime datetime, int offset)
		{
			if (datetime.Kind != DateTimeKind.Utc)
			{
				throw new Exception("Needs to be UTC");
			}

			var local = new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, DateTimeKind.Local);
			local = local.Add(TimeSpan.FromHours(offset));
			return local;
		}
	}
}
