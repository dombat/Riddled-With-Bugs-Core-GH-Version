namespace RiddledWithBugsCoreLibrary
{
    public class MassiveOverflow
    {
        private decimal MyFirstBigNumber { get; set; } = decimal.MaxValue;
        private decimal MySecondBigNumber { get; set; } = decimal.MaxValue;

        private MassiveOverflow()
        {
            var superBig = MyFirstBigNumber + MySecondBigNumber;
        }
    }
}