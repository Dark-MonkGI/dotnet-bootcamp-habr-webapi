namespace Habr.Common
{
    public static class Constants
    {
        public static class Log
        {
            public const string LogFilePath = "logs/log.txt";
        }

        public static class Post
        {
            public const int TitleMaxLength = 200;
            public const int TextMaxLength = 2000;
        }

        public static class Comment
        {
            public const int TextMaxLength = 200;
        }

        public static class User
        {
            public const int NameMaxLength = 100;
            public const int EmailMaxLength = 200;
            public const bool DefaultIsEmailConfirmed = false;
            public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        }
    }
}
