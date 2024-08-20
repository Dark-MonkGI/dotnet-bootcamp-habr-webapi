namespace Habr.Common
{
    public static class Constants
    {
        public static class RatingConstants
        {
            public const int MinValue = 1;
            public const int MaxValue = 5;
        }

        public static class Pagination
        {
            public const int DefaultPageNumber = 1;
            public const int DefaultPageSize = 10;
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public static class Policies
        {
            public const string UserPolicy = "UserPolicy";
            public const string AdminPolicy = "AdminPolicy";
        }
        public static class Tags
        {
            public const string PostsTag = "Posts";
            public const string UsersTag = "Users";
            public const string CommentsTag = "Comments";
            public const string RatingsTag = "Ratings";
            public const string AdminTag = "Admin rights";
        }
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
            public const int PasswordMinLength = 8;
            public const int SecurityStampMaxLength = 100;
            public const int RefreshTokenMaxLength = 500;           
            public const bool DefaultIsEmailConfirmed = false;
            public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        }
    }
}
