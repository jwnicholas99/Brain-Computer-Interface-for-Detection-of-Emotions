using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace  BCILib.Util
{
	/// <summary>
	/// Summary description for HRTimer.
	/// </summary>
	public class HRTimer
	{
		private static long Freq = 0;

		[DllImport("winmm.dll")]
		private static extern uint timeBeginPeriod(uint period);
		[DllImport("winmm.dll")] private static extern uint timeEndPeriod(uint period);
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(
			out long PerformanceCount);
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(
			out long Frequency);

		private static HRTimer myTimer = null;

		static HRTimer()
		{
			Console.WriteLine("HRTimer initailized.");
			timeBeginPeriod(1);
			QueryPerformanceFrequency(out Freq);

			myTimer = new HRTimer();
		}

		~HRTimer()
		{
			timeEndPeriod(1);
			try {
				Console.WriteLine("HRTimer restored");
			} catch (Exception) {};
		}

		static public long GetTimestamp()
		{
			long result;
			QueryPerformanceCounter(out result);
			return result;
		}

		static public int DeltaMilliseconds(long earlyTimestamp, long
			lateTimestamp)
		{
			return (int) (((lateTimestamp - earlyTimestamp) * 1000 + (Freq >> 1))
				/ Freq);
		}

		static public int DeltaMilliseconds(long earlyTimestamp)
		{
			return (int) (((GetTimestamp() - earlyTimestamp) * 1000 + (Freq >> 1))
				/ Freq);
		}

		static public void Wait(int delay)
		{
			System.Threading.Thread.Sleep(delay);
		}
	}
}
