namespace Minigun.Utilities
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime to a human-readable "time ago" string.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert.</param>
        /// <param name="currentDateTime">The current DateTime for reference. Defaults to DateTime.UtcNow.</param>
        /// <returns>A human-readable string representing the time difference.</returns>
        public static string ToHumanReadableTimeAgo(this DateTime dateTime, DateTime? currentDateTime = null)
        {
            var now = currentDateTime ?? DateTime.UtcNow;
            var timeSpan = now - dateTime;

            if (timeSpan.TotalSeconds < 60)
            {
                return "A few seconds ago";
            }

            if (timeSpan.TotalMinutes < 60)
            {
                var minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "A minute ago" : $"{minutes} minutes ago";
            }
            if (timeSpan.TotalHours < 24)
            {
                var hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "An hour ago" : $"{hours} hours ago";
            }
            switch (timeSpan.TotalDays)
            {
                case < 30:
                {
                    var days = (int)timeSpan.TotalDays;
                    return days == 1 ? "Yesterday" : $"{days} days ago";
                }
                case < 365:
                {
                    var months = (int)(timeSpan.TotalDays / 30);
                    return months == 1 ? "A month ago" : $"{months} months ago";
                }
                default:
                {
                    var years = (int)(timeSpan.TotalDays / 365);
                    return years == 1 ? "A year ago" : $"{years} years ago";
                }
            }
        }
    }
}