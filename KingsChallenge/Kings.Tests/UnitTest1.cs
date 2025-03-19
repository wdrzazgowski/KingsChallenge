namespace Kings.Tests;

using KingsChallenge;

public class UnitTest1
{
    [Fact]
    public void FirstNameShouldReturnEmptyIfNameEmpty()
    {
        Monarch kingWithNoName = new Monarch(new MonarchDto(), null);
        kingWithNoName._monarchData.nm = "";

        Assert.Equal("", kingWithNoName.FirstName());
    }

    [Fact]
    public void FirstNameShouldThrowExceptionIfNameNull()
    {
        Monarch kingWithNoName = new Monarch(new MonarchDto(), null);
        kingWithNoName._monarchData.nm = null;

        Assert.Throws<NullReferenceException>(() => kingWithNoName.FirstName());
    }

    [Fact]
    public void FirstNameShouldBeSameAsFirstSplitItem()
    {
        Monarch k = new Monarch(new MonarchDto(), null);
        k._monarchData.nm = "Mieszko I";

        Assert.Equal("Mieszko", k.FirstName());
    }
}