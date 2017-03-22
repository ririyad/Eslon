namespace Eslon
{
    internal static class COM
    {
        public static string Express(string format, params object[] args)
        {
            API.Check(format, nameof(format));
            API.Check(args, nameof(args));

            return ExtraRender.Detonate(format, args, '%', null);
        }
    }
}
