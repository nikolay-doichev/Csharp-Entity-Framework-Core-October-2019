namespace PetStore.Models.DataValidation
{
    public static class DataValidation
    {
        public const int NameMaxLenght = 30;

        public static class Category
        {
            public const int DescriptionMaxLengh = 1000;
        }
        public static class User
        {
            public const int EmailMaxLenght = 100;
        }
        public static class Pet
        {
            public const int DescriptionMaxLengh = 1000;
        }

        public static class Toy
        {
            public const int DescriptionMaxLengh = 1000;
        }
    }
}
