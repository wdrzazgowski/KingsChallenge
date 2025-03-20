namespace Kings.Tests;

using KingsChallenge;

public class UnitTest1
{
    [Fact]
    public void FirstNameShouldReturnEmptyIfNameEmpty()
    {
        Monarch kingWithNoName = new Monarch(
            new MonarchDto()
            { 
                id = 1, 
                nm = "", 
                cty = "", 
                hse = "", 
                yrs = "950-960" 
            },
            null);
 
        Assert.Equal("", kingWithNoName.FirstName);
    }

    [Fact]
    public void FirstNameShouldThrowExceptionIfNameNull()
    {
        Monarch kingWithNoName = new Monarch(new MonarchDto(){ id = 1, nm = null, cty = "", hse = "", yrs = "950-960" }, null);
        
        Assert.Throws<NullReferenceException>(() => kingWithNoName.CalculateFirstName());
    }

    [Fact]
    public void FirstNameShouldBeSameAsFirstSplitItem()
    {
        Monarch k = new Monarch(new MonarchDto(){ id = 1, nm = "", cty = "", hse = "", yrs = "950-960" }, null);
        k._monarchData.nm = "Mieszko I";
        k._monarchData.yrs = "2000-1000";
        k.CalculateFirstName();

        Assert.Equal("Mieszko", k.FirstName);
    }
}