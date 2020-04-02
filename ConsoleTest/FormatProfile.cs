namespace ConsoleTest
{
    public static class FormatProfile
    {
        // use Translate#RarityS ?
        public static Color Rarity2Color(string rarity)
        {
            switch (rarity)
            {
                case "header-purple":
                    return Color.Purple;
                case "header-he":
                    return Color.Orange;
                case "header-named":
                    return Color.Named;
                case "header-gs":
                    return Color.Green;
                default:
                    return Color.Default;
            }
        }
    }
}
