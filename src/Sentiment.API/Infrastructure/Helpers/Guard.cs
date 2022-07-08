namespace Sentiment.API.Infrastructure.Helpers
{
    public static class Guard
    {
        public static void ThrowIfNullOrEmpty(string? argument, string? paramName = default)
        {
            if(string.IsNullOrEmpty(argument))
            {
                if(paramName != null)
                    throw new ArgumentNullException(paramName);

                throw new ArgumentNullException(nameof(argument));
            }                
        }
    }
}
