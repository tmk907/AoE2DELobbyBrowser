namespace AoE2DELobbyBrowserAvalonia.Converters
{
    public static class Converters
    {
        public static BoolToFontWeightConverter FontWeightConverter { get; } = new BoolToFontWeightConverter();
        public static EnumToBooleanConverter EnumToBooleanConverter { get; } = new EnumToBooleanConverter();
    }
}
