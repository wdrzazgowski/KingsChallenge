namespace Kings.Tests;

using KingsChallenge;

public class UnitTest1
{
    [Fact]
    public void FirstNameShouldReturnEmptyIfNameEmpty()
    {
        KingRaw kingWithNoName = new KingRaw();
        kingWithNoName.nm = "";

        Assert.Equal(kingWithNoName.FirstName(), "");
    }

    [Fact]
    public void FirstNameShouldThrowExceptionIfNameNull()
    {
        KingRaw kingWithNoName = new KingRaw();
        kingWithNoName.nm = null;

        Assert.Throws<NullReferenceException>(() => kingWithNoName.FirstName());
    }

    [Fact]
    public void FirstNameShouldBeSameAsFirstSplitItem()
    {
        KingRaw k = new KingRaw();
        k.nm = "Mieszko I";

        Assert.Equal(k.FirstName(), "Mieszko");
    }
}