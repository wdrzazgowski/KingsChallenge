namespace Kings.Tests;

using KingsChallenge;

public class UnitTest1
{
    [Fact]
    public void FirstNameShouldReturnEmptyIfNameEmpty()
    {
        Monarch kingWithNoName = new Monarch();
        kingWithNoName.nm = "";

        Assert.Equal("", kingWithNoName.FirstName());
    }

    [Fact]
    public void FirstNameShouldThrowExceptionIfNameNull()
    {
        Monarch kingWithNoName = new Monarch();
        kingWithNoName.nm = null;

        Assert.Throws<NullReferenceException>(() => kingWithNoName.FirstName());
    }

    [Fact]
    public void FirstNameShouldBeSameAsFirstSplitItem()
    {
        Monarch k = new Monarch();
        k.nm = "Mieszko I";

        Assert.Equal("Mieszko", k.FirstName());
    }
}