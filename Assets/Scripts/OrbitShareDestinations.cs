public static class OrbitShareDestinations
{
    public const string Seoul = "Seoul";
    public const string NewYork = "New York";
    public const string London = "London";
    public const string Tokyo = "Tokyo";
    public const string Paris = "Paris";
    public const string Sydney = "Sydney";
    public const string LosAngeles = "Los Angeles";
    public const string DefaultTargetName = NewYork;

    public static readonly string[] Names =
    {
        Seoul,
        NewYork,
        London,
        Tokyo,
        Paris,
        Sydney,
        LosAngeles
    };

    public static readonly string[] DemoTargetNames =
    {
        NewYork,
        London,
        Seoul,
        Tokyo,
        Paris,
        Sydney,
        LosAngeles
    };

    public static string Label(string targetName)
    {
        if (targetName == Seoul)
        {
            return "서울";
        }
        if (targetName == NewYork)
        {
            return "뉴욕";
        }
        if (targetName == London)
        {
            return "런던";
        }
        if (targetName == Tokyo)
        {
            return "도쿄";
        }
        if (targetName == Paris)
        {
            return "파리";
        }
        if (targetName == Sydney)
        {
            return "시드니";
        }
        if (targetName == LosAngeles)
        {
            return "LA";
        }
        if (targetName == "None")
        {
            return "없음";
        }
        return targetName;
    }

    public static string TargetNameFromLabel(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            return string.Empty;
        }
        if (label == "서울" || label == Seoul)
        {
            return Seoul;
        }
        if (label == "뉴욕" || label == NewYork)
        {
            return NewYork;
        }
        if (label == "런던" || label == London)
        {
            return London;
        }
        if (label == "도쿄" || label == Tokyo)
        {
            return Tokyo;
        }
        if (label == "파리" || label == Paris)
        {
            return Paris;
        }
        if (label == "시드니" || label == Sydney)
        {
            return Sydney;
        }
        if (label == "LA" || label == LosAngeles)
        {
            return LosAngeles;
        }
        return string.Empty;
    }

    public static bool IsKnown(string targetName)
    {
        for (int i = 0; i < Names.Length; i++)
        {
            if (Names[i] == targetName)
            {
                return true;
            }
        }
        return false;
    }
}
