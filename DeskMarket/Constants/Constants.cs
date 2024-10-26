namespace DeskMarket.Constants
{
    public static class Constants
    {
        public const int ProductNameMinLength = 2;
        public const int ProductNameMaxLength = 60;

        public const int DescriptionMinLength = 10;
        public const int DescriptionMaxLength = 250;

        public const decimal PriceMinNumber = 1.00m;
        public const decimal PriceMaxNumber = 3000.00m;

        public const string ProperDateFormat = "dd-MM-yyyy";

        public const int CategoryMinName = 3;
        public const int CategoryMaxName = 20;
    }
}
