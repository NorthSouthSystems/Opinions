public class T_EnumX
{
    [Fact]
    public void Basic()
    {
        EnumX.GetValues<DayOfWeek>().Should().Equal(
            new[]
            {
                DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                DayOfWeek.Friday, DayOfWeek.Saturday
            });
    }
}
