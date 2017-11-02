using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTec.TA.Library;
using YsrisCoreLibrary.Models;

namespace ysriscorelibrary.Helpers
{
public static class TupleExtensions
{	
	public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<TimeSerieItem> data)
		=> Rebase100(data.Select(a => new Tuple<DateTime, decimal>(a.Date, (decimal)a.typicalPrice)));

	public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<Tick> data)
		=> Rebase100(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)));

	public static IEnumerable<Tuple<DateTime, decimal>> Rebase100(this IEnumerable<Tuple<DateTime, decimal>> data)
	{
		var first = data.First().Item2;
		return
			from x in data
			select new Tuple<DateTime, decimal>(x.Item1, 1 + x.Item2 / first);
	}

	public static IEnumerable<Tuple<DateTime, decimal>> GetEma(this IEnumerable<Tuple<DateTime, decimal>> data, int window)
	{
		double[]
			inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
			, output = new double[inputClose.Length];
		int outBegIdx, outNbElement;

		TicTacTec.TA.Library.Core.Ema(
			0,
			inputClose.Length - 1,
			inputClose,
			window,
			out outBegIdx,
			out outNbElement,
			output
		);

		return
			output
			.Take(outNbElement)
			.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
	}

    public static IEnumerable<Tuple<DateTime, decimal>> GetSma(this IEnumerable<TimeSerieItem> data, int window)
        => GetSma(data.Select(a => new Tuple<DateTime, decimal>(a.Date, (decimal)a.typicalPrice)), window);
        
	public static IEnumerable<Tuple<DateTime, decimal>> GetSma(this IEnumerable<Tuple<DateTime, decimal>> data, int window)
	{
		double[]
			inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
			, output = new double[inputClose.Length];
		int outBegIdx, outNbElement;

		TicTacTec.TA.Library.Core.Sma(
			0,
			inputClose.Length - 1,
			inputClose,
			window,
			out outBegIdx,
			out outNbElement,
			output
		);

		return
			output
			.Take(outNbElement)
			.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
	}


	public static IEnumerable<MacdTick> GetMacd(this IEnumerable<TimeSerieItem> data, int fast = 12, int slow = 26, int signal = 9)
		=> GetMacd(data.Select(a => new Tuple<DateTime, decimal>(a.Date, (decimal)a.typicalPrice)), fast, slow, signal);

	public static IEnumerable<MacdTick> GetMacd(this IEnumerable<Tick> data, int fast = 12, int slow = 26, int signal = 9)
		=> GetMacd(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)), fast, slow, signal);


	public static IEnumerable<MacdTick> GetMacd(this IEnumerable<Tuple<DateTime, decimal>> data, int fast = 12, int slow = 26, int signal = 9)
	{
		double[]
			inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
			, outMACD = new double[inputClose.Length]
			, outMACDSignal = new double[inputClose.Length]
			, outMACDHist = new double[inputClose.Length]
			;
		int outBegIdx, outNbElement;

		TicTacTec.TA.Library.Core.Macd(
			0,
			inputClose.Length - 1,
			inputClose,
			fast,
			slow,
			signal,
			out outBegIdx,
			out outNbElement,
			outMACD, outMACDSignal, outMACDHist
		);

		var outMACD2 = outMACD.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
		var outMACDSignal2 = outMACDSignal.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
		var outMACDHist2 = outMACDHist.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
		var resultofmacd =
			from x in data
			join y in outMACD2 on x.Item1.Date equals y.Item1.Date
			join z in outMACDSignal2 on x.Item1 equals z.Item1
			join u in outMACDHist2 on x.Item1 equals u.Item1
			select new MacdTick
			{
				DateTime = x.Item1,
				MACD = y.Item2,
				Signal = z.Item2,
				Hist = u.Item2

			};


		return resultofmacd;
	}

	public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<TimeSerieItem> data, int period = 14)
		=> GetRsi(data.Select(a => new Tuple<DateTime, decimal>(a.Date, (decimal)a.typicalPrice)), period);

	public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<Tick> data, int period = 14)
		=> GetRsi(data.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close)), period);

	public static IEnumerable<Tuple<DateTime, decimal>> GetRsi(this IEnumerable<Tuple<DateTime, decimal>> data, int period = 14)
	{
		double[]
			inputClose = data.Select(a => Convert.ToDouble(a.Item2)).ToArray()
			, output = new double[inputClose.Length]
			;
		int outBegIdx, outNbElement;

		TicTacTec.TA.Library.Core.Rsi(
			0,
			inputClose.Length - 1,
			inputClose,
			period,
			out outBegIdx,
			out outNbElement,
			output
		);



		return
			output
			.Take(outNbElement)
			.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)));
	}

	public static IEnumerable<Tuple<DateTime, decimal>> GetMfi(this IEnumerable<Tick> data, int window = 14)
	{
		double[]
			inputHigh = data.Select(a => Convert.ToDouble(a.High)).ToArray(),
			inputLow = data.Select(a => Convert.ToDouble(a.Low)).ToArray(),
			inputClose = data.Select(a => Convert.ToDouble(a.Close)).ToArray(),
			inputVolume = data.Select(a => Convert.ToDouble(a.Volume)).ToArray()
			, output = new double[inputClose.Length];
		int outBegIdx, outNbElement;

		TicTacTec.TA.Library.Core.Mfi(
			0,
			inputClose.Length - 1,
			inputHigh,
			inputLow,
			inputClose,
			inputVolume,
			window,
			out outBegIdx,
			out outNbElement,
			output
		);

		return
			output
			.Take(outNbElement)
			.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
	}

	public static IEnumerable<RsiTick> GetStochRSI(this IEnumerable<Tick> data, int window = 14)
	{
		double[]
			inputClose = data.Select(a => Convert.ToDouble(a.Close)).ToArray()
			, outputfastk = new double[inputClose.Length]
			, outputfastd = new double[inputClose.Length]
			;
		int outBegIdx, outNbElement;

		int fastKPeriod = 5, fastDPeriod = 3;

		TicTacTec.TA.Library.Core.StochRsi(
			0,
			inputClose.Length - 1,
			inputClose,
			window,
			fastKPeriod,
			fastDPeriod,
			Core.MAType.Sma,
			out outBegIdx,
			out outNbElement,
			outputfastk,
			outputfastd
		);
		var outputfastk2 = outputfastk.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
		var outputfastd2 = outputfastd.Take(outNbElement).Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.DateTime, Convert.ToDecimal(a)));
		var result =
			from x in data
			join y in outputfastk2 on x.DateTime equals y.Item1
			join z in outputfastd2 on x.DateTime equals z.Item1
			select new RsiTick
			{
				DateTime = x.DateTime,
				K = y.Item2,
				D = z.Item2,

			};
		return result;
	}

	/// <summary>
	/// Get a moving window of a time serie list
	/// </summary>
	/// <param name="data">data to treat</param>
	/// <param name="window">window size</param>
	/// <returns></returns>
	public static Dictionary<DateTime, IEnumerable<Tick>> GetMovingWindow(this IEnumerable<Tick> data, int window)
	{
		data = data.OrderBy(a => a.DateTime).ToList();

		var x = new Dictionary<DateTime, IEnumerable<Tick>>();
		for (int i = 0; i <= data.Count() - window; i++)
		{
			var outData = data.Skip(i).Take(window).ToList();
			x.Add(outData.Last().DateTime, outData);
		}

		return x;
	}

	public static IEnumerable<IchimokuTick> GetIchimoku(this IEnumerable<Tick> data, int conversionLinePeriod = 9, int baseLinePeriod = 26, int laggingSpan2Period = 52, int displacement = 26)
	{
		var data2 = data.ToList();


		var tenkanSen = data2.GetMovingWindow(conversionLinePeriod).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

		var kijuSen = data2.GetMovingWindow(baseLinePeriod).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

		var senkouSpanA =
				(from x in tenkanSen
				 join y in kijuSen on x.Key equals y.Key into y2
				 from y in y2.DefaultIfEmpty()
				 select new { key = x.Key, value = (x.Value + y.Value) / 2 })
				.ToDictionary(a => a.key, a => a.value);

		var senkouSpanB = data2.GetMovingWindow(laggingSpan2Period).ToDictionary(a => a.Key, a => Convert.ToDecimal((a.Value.Max(b => b.High) + a.Value.Min(b => b.Low)) / 2));

		var chikouSpan = new Dictionary<DateTime, double?>();
		for (int i = 0; i < data2.Count() - displacement; i++)
			chikouSpan.Add(data2[i].DateTime, Convert.ToDouble(data2[i + displacement].Close));
		for (int i = data2.Count() - displacement; i < data2.Count(); i++)
			chikouSpan.Add(data2[i].DateTime, double.NaN);

		var qry =
		(
			from x in data.Skip(laggingSpan2Period)
			select new IchimokuTick
			{
				DateTime = x.DateTime,


				Open = x.Open,
				High = x.High,
				Low = x.Low,
				Close = x.Close,
				Volume = x.Volume,


				TenkanSen = tenkanSen.ContainsKey(x.DateTime) ? tenkanSen[x.DateTime] as Nullable<decimal> : null,
				KijuSen = kijuSen.ContainsKey(x.DateTime) ? kijuSen[x.DateTime] as Nullable<decimal> : null,
				SenkouSpanA = senkouSpanA.ContainsKey(x.DateTime) ? senkouSpanA[x.DateTime] as Nullable<decimal> : null,
				SenkouSpanB = senkouSpanB.ContainsKey(x.DateTime) ? senkouSpanB[x.DateTime] as Nullable<decimal> : null,
				ChikouSpan = chikouSpan.ContainsKey(x.DateTime) ? chikouSpan[x.DateTime] as Nullable<double> : null,
			}
		).ToList();

		return qry;
	}

	public static IEnumerable<Tuple<Tick, double>> Mom(this IEnumerable<Tick> data, int windowSize)
	{
		var inputClose = data.Select(a => (float)a.Close).ToArray();
		var output = new Double[inputClose.Length];
		int outBegIdx, outNbElement;
		TicTacTec.TA.Library.Core.Mom(startIdx: 0, endIdx: inputClose.Length - 1, inReal: inputClose, optInTimePeriod: windowSize, outBegIdx: out outBegIdx, outNBElement: out outNbElement, outReal: output);
		return
			output
			.Take(outNbElement)
			.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<Tick, double>(b, a));
	}

	public static IEnumerable<Tuple<DateTime, decimal>> GetStDev(this IEnumerable<Tuple<DateTime, decimal>> data, int windowSize)
	{
		var inputClose = data.Select(a => (float)a.Item2).ToArray();
		var output = new Double[inputClose.Length];
		int outBegIdx, outNbElement;
		TicTacTec.TA.Library.Core.StdDev
		(
			startIdx: 0,
			endIdx: inputClose.Length - 1,
			inReal: inputClose,
			optInTimePeriod: windowSize,
			optInNbDev: 1,
			outBegIdx: out outBegIdx,
			outNBElement: out outNbElement,
			outReal: output
		);
		return output
		.Take(outNbElement)
		.Zip(data.Skip(outBegIdx).Take(outNbElement), (a, b) => new Tuple<DateTime, decimal>(b.Item1, Convert.ToDecimal(a)))
		;
	}

	public static IEnumerable<Tuple<DateTime, decimal>> GetBollingerBand(this Dictionary<Tuple<DateTime, DateTime>, Tick> me)
	{
		var p = 65;

		var fuu = me.Select(a => a.Value).Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.Close));

		return
			from x in fuu
			join sma in fuu.GetSma(p).ToList() on x.Item1 equals sma.Item1
			join stdev in fuu.GetStDev(p).ToList() on x.Item1 equals stdev.Item1
			let standardDev = stdev.Item2 + 0.000000000001m
			let cctbbo = 100m * (x.Item2 + 2m * standardDev - sma.Item2) / (4m * standardDev)
			select new Tuple<DateTime, decimal>(x.Item1, Convert.ToDecimal(cctbbo));
	}

	public static Tuple<DateTime, double, double, double> CctBboSignalOfLastTick(this Dictionary<Tuple<DateTime, DateTime>, Tick> me)
	{
		var raw = me.Select(a => a.Value);
		var ts1 = me.GetBollingerBand().ToList();
		var ts2 = ts1.GetEma(30);

		// 1. Fist pass : CCTBO crossover
		var bfr =
			(
				from x in raw
				join y in ts1 on x.DateTime equals y.Item1
				join z2 in ts2 on x.DateTime equals z2.Item1
				let emacctbbo = z2.Item2
				let tts1 = y.Item2
				let tts2 = emacctbbo
				select new
				{
					x.DateTime,
					tts1,
					tts2,
					signal =
						tts1 >= tts2
							? tts1 > 100
								? 2 /*bullish cross and BUY emacctbbo*/
								: 1 /*bullish cross and HOLD emacctbbo*/
							: tts1 < 0
								? -1 /*bearish cross and SELL emacctbbo*/
								: 0 /*bearish cross and HOLD emacctbbo*/
				}).ToList();

		// Smooth with SMA 5
		var secondSmoothSmaPeriod = 5;
		var smoothed = bfr.Select(a => new Tuple<DateTime, decimal>(a.DateTime, a.signal)).GetSma(secondSmoothSmaPeriod);

		// Get the signal 
		var qry =
		(
			from ts in raw
			join y in smoothed on ts.DateTime equals y.Item1
			select new
			{
				ts.DateTime,
				smoothedsignal = y.Item2,
				smoothedsignalbinary =
					y.Item2 >= 2
						? 2
					: y.Item2 >= 0.8m
						? 1
					: y.Item2 <= -1
						? -1
					: 0
			}
		).ToList();

		if (!qry.Any())
		{
			return null;
		}
		var lastEntity = qry.Last();
		return new Tuple<DateTime, double, double, double>(
			lastEntity.DateTime,
			Convert.ToDouble(bfr.Single(a => a.DateTime == lastEntity.DateTime).tts1),
			Convert.ToDouble(bfr.Single(a => a.DateTime == lastEntity.DateTime).tts2),
			Convert.ToDouble(qry.Single(a => a.DateTime == lastEntity.DateTime).smoothedsignalbinary)
		);
	}
}


public class MacdTick
{
	public DateTime DateTime { get; set; }
	public decimal MACD { get; set; }
	public decimal Signal { get; set; }
	public decimal Hist { get; set; }
}

public class RsiTick
{
	public DateTime DateTime { get; set; }
	public decimal K { get; set; }
	public decimal D { get; set; }
}



public class Tick
{
	public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		return dtDateTime;
	}

	public Tick()
	{

	}

	public Tick(List<decimal> a)
	{
		DateTime = UnixTimeStampToDateTime((Convert.ToDouble(a[0])));
		Open = a[1];
		High = a[2];
		Low = a[3];
		Close = a[4];
		Volume = a[5];

	}

	public DateTime DateTime { get; set; }
	public decimal Open { get; set; }
	public decimal High { get; set; }
	public decimal Low { get; set; }
	public decimal Close { get; set; }
	public decimal Volume { get; set; }
}

public class IchimokuTick : Tick
{
	public decimal? TenkanSen { get; internal set; }
	public decimal? KijuSen { get; internal set; }
	public decimal? SenkouSpanA { get; internal set; }
	public decimal? SenkouSpanB { get; internal set; }
	public double? ChikouSpan { get; internal set; }
}
}