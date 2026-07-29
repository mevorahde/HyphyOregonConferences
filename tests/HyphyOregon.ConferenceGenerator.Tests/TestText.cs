namespace HyphyOregon.ConferenceGenerator.Tests;

internal static class TestText
{
    public static string ToLf(string value) =>
        value
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
}

[TestClass]
public sealed class TestTextTests
{
    [TestMethod]
    public void ToLfNormalizesCrlfAndLoneCrWithoutChangingLf()
    {
        const string expected = "first\nsecond\nthird\n";

        Assert.AreEqual(expected, TestText.ToLf(expected));
        Assert.AreEqual(expected, TestText.ToLf("first\r\nsecond\r\nthird\r\n"));
        Assert.AreEqual(expected, TestText.ToLf("first\rsecond\rthird\r"));
    }
}
