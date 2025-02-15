public static class CurrencyFormatter
{
    public static string FormatCurrency(long amount)
    {
        if (amount >= 1000000000000)
        {
            return (amount / 1000000000000f).ToString("0.###") + "T";
        }
        else if (amount >= 1000000000)
        {
            return (amount / 1000000000f).ToString("0.###") + "B";
        }
        else if (amount >= 1000000)
        {
            return (amount / 1000000f).ToString("0.###") + "M";
        }
        else if (amount >= 1000)
        {
            return (amount / 1000f).ToString("0.###") + "K";
        }
        else
        {
            return amount.ToString();
        }
    }
}