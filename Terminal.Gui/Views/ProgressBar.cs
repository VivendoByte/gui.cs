using System;
namespace Terminal.Gui {

	/// <summary>
	/// Progress bar can indicate progress of an activity visually.
	/// </summary>
	/// <remarks>
	///   <para>
	///     The progressbar can operate in two modes, percentage mode, or
	///     activity mode.  The progress bar starts in percentage mode and
	///     setting the Fraction property will reflect on the UI the progress 
	///     made so far.   Activity mode is used when the application has no 
	///     way of knowing how much time is left, and is started when you invoke
	///     the Pulse() method.    You should call the Pulse method repeatedly as
	///     your application makes progress.
	///   </para>
	/// </remarks>
	public class ProgressBar : View {
		bool isActivity;
		int activityPos, delta;

		/// <summary>
		/// Gets or sets the rune used to draw the progress bar indicator
		/// </summary>
		public Rune Filler { get; set; } = Driver.Stipple;

		/// <summary>
		/// Gets or sets the rune used to draw the remaining
		/// </summary>
		public Rune RemainingFiller { get; set; } = new Rune(' ');

		/// <summary>
		/// Turn on/off the percentage indicator drawn over the progress bar
		/// </summary>
		public bool ShowPercentage { get; set; }

		/// <summary>
		/// Gets or sets the label shown when percentage is 100% (progress bar full)
		/// </summary>
		public string FinalLabel { get; set; } = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode with an absolute position and size.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public ProgressBar(Rect rect) : base(rect)
		{
			CanFocus = false;
			fraction = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode and uses relative layout.
		/// </summary>
		public ProgressBar() : base()
		{
			CanFocus = false;
			fraction = 0;
		}

		float fraction;

		/// <summary>
		/// Gets or sets the progress indicator fraction to display, must be a value between 0 and 1.
		/// </summary>
		/// <value>The fraction representing the progress.</value>
		public float Fraction {
			get => fraction;
			set {
				fraction = value;
				isActivity = false;
				SetNeedsDisplay();
			}
		}

		/// <summary>
		/// Notifies the progress bar that some progress has taken place.
		/// </summary>
		/// <remarks>
		/// If the ProgressBar is is percentage mode, it switches to activity
		/// mode.   If is in activity mode, the marker is moved.
		/// </remarks>
		public void Pulse()
		{
			if (!isActivity) {
				isActivity = true;
				activityPos = 0;
				delta = 1;
			} else {
				activityPos += delta;
				if (activityPos < 0) {
					activityPos = 1;
					delta = 1;
				} else if (activityPos >= Frame.Width) {
					activityPos = Frame.Width - 2;
					delta = -1;
				}
			}

			SetNeedsDisplay();
		}

		public override void Redraw(Rect region)
		{
			Driver.SetAttribute(ColorScheme.Normal);

			int top = Frame.Width;
			if (isActivity) {
				Move(0, 0);
				for (int i = 0; i < top; i++)
					if (i == activityPos)
						Driver.AddRune(this.Filler);
					else
						Driver.AddRune(this.RemainingFiller);
			} else {
				Move(0, 0);
				int mid = (int)(fraction * top);
				int i;
				for (i = 0; i < mid; i++)
					Driver.AddRune(this.Filler);
				for (; i < top; i++)
					Driver.AddRune(this.RemainingFiller);


				if (this.ShowPercentage) {
					float value = fraction * 100;
					string output;

					if (value != 100F) {
						output = $" {value}% ";
					} else {
						output = this.FinalLabel;
					}

					int length = output.Length;
					int x = Math.Max(0, (int)(((double)mid / 2.0) - (double)length));

					this.Move(x, 0);
					Driver.AddStr(output);
				}

				this.SetNeedsDisplay();
			}
		}
	}
}