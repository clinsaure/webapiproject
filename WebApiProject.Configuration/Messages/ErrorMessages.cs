namespace WebApiProject.Configuration.Messages
{
    public static class ErrorMessages
    {
        public static class Generic
        {
            public static string ObjectNotFound = "Object not found";
            public static string InvalidRequest = "Invalid Request";
            public static string TypeBadRequest = "Bad Request";
            public static string InvalidPayload = "Invalid Payload";
            public static string UnableToProcess = "Unable to process request";
            public static string SomethingWentWrong = "Something went wrong, please try again later";
        }

        public static class Profile
        {
            public static string UserNotFound = "User not found";           
        }

        public static class Users
        {
            public static string UserNotFound = "User not found";
        }
    }
}