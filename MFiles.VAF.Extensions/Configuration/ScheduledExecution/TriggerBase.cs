﻿using MFiles.VAF.Configuration;
using System;
using System.Runtime.Serialization;

namespace MFiles.VAF.Extensions.ScheduledExecution

{
	/// <summary>
	/// Class used for configuration purposes.
	/// </summary>
	[DataContract]
	public class Trigger
		: TriggerBase
	{
		/// <summary>
		/// The type of trigger this is (e.g. Daily, Weekly).
		/// </summary>
		[DataMember]
		public new ScheduleTriggerType Type
		{
			get => base.Type;
			protected set => base.Type = value;
		}

		[DataMember]
		[JsonConfEditor
		(
			Label = ResourceMarker.Id + nameof(Resources.Configuration.General_Configuration),
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Daily' }"
		)]
		public DailyTrigger DailyTriggerConfiguration { get; set; }
			= new DailyTrigger();

		[DataMember]
		[JsonConfEditor
		(
			Label = ResourceMarker.Id + nameof(Resources.Configuration.General_Configuration),
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Weekly' }"
		)]
		public WeeklyTrigger WeeklyTriggerConfiguration { get; set; }
			= new WeeklyTrigger();

		[DataMember]
		[JsonConfEditor
		(
			Label = ResourceMarker.Id + nameof(Resources.Configuration.General_Configuration),
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Monthly' }"
		)]
		public DayOfMonthTrigger DayOfMonthTriggerConfiguration { get; set; }
			= new DayOfMonthTrigger();

		/// <inheritdoc />
		public override DateTime? GetNextExecution(DateTime? after = null)
		{
			switch (this.Type)
			{
				case ScheduleTriggerType.Daily:
					return this.DailyTriggerConfiguration?.GetNextExecution(after);
				case ScheduleTriggerType.Weekly:
					return this.WeeklyTriggerConfiguration?.GetNextExecution(after);
				case ScheduleTriggerType.Monthly:
					return this.DayOfMonthTriggerConfiguration?.GetNextExecution(after);
				default:
					return null;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			switch (this.Type)
			{
				case ScheduleTriggerType.Daily:
					return this.DailyTriggerConfiguration?.ToString();
				case ScheduleTriggerType.Weekly:
					return this.WeeklyTriggerConfiguration?.ToString();
				case ScheduleTriggerType.Monthly:
					return this.DayOfMonthTriggerConfiguration?.ToString();
				default:
					return null;
			}
		}

		public Trigger()
		{
		}
		public Trigger(ScheduleTriggerType type)
			: this()
		{
			this.Type = type;
		}
		public Trigger(TriggerBase triggerBase)
			: this()
		{
			// Sanity.
			if (null == triggerBase)
				throw new ArgumentNullException(nameof(triggerBase));

			// Note the order here is important, as some of the trigger configurations inherit from others.
			if (triggerBase is DayOfMonthTrigger monthlyTrigger)
			{
				this.Type = ScheduleTriggerType.Monthly;
				this.DayOfMonthTriggerConfiguration = monthlyTrigger;
			}
			else if (triggerBase is WeeklyTrigger weeklyTrigger)
			{
				this.Type = ScheduleTriggerType.Weekly;
				this.WeeklyTriggerConfiguration = weeklyTrigger;
			}
			else if (triggerBase is DailyTrigger dailyTrigger)
			{
				this.Type = ScheduleTriggerType.Daily;
				this.DailyTriggerConfiguration = dailyTrigger;
			}
			else
				throw new ArgumentException
				(
					$"Trigger configuration type {triggerBase.GetType().FullName} was not supported", 
					nameof(triggerBase)
				);

		}
	}
	public abstract class TriggerBase
	{
		/// <summary>
		/// The type of trigger this is (e.g. Daily, Weekly).
		/// </summary>
		public ScheduleTriggerType Type { get; set; } = ScheduleTriggerType.Unknown;

		/// <summary>
		/// Gets the next execution datetime for this trigger.
		/// </summary>
		/// <param name="after">The time after which the schedule should run.  Defaults to now (i.e. next-run time) if not provided.</param>
		/// <returns>The next execution time.</returns>
		public abstract DateTime? GetNextExecution(DateTime? after = null);
	}
}