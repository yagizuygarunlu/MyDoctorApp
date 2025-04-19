namespace WebApi.Common.Localization
{
    public static class LocalizationKeys
    {
        public static class Common
        {
            public const string Success = "Common.Success";
            public const string Error = "Common.Error";
        }
        public static class Appointments
        {
            public const string NotFound = "Appointments.NotFound";
            public const string Created = "Appointments.Created";
            public const string Updated = "Appointments.Updated";
            public const string Deleted = "Appointments.Deleted";
            public const string InvalidDate = "Appointments.InvalidDate";
            public const string InvalidTime = "Appointments.InvalidTime";
            public const string InvalidStatus = "Appointments.InvalidStatus";
            public const string Approved = "Appointments.Approved";
            public const string Rejected = "Appointments.Rejected";
            public const string Cancelled = "Appointments.Cancelled";
            public const string DoctorRequired = "Appointments.DoctorRequired";
            public const string DoctorIdMustBeGreaterThanZero = "Appointments.DoctorIdMustBeGreaterThanZero";
            public const string DateRequired = "Appointments.DateRequired";
            public const string DateMustBeInFuture = "Appointments.DateMustBeInFuture";
        }
        public static class Auth
        {
            public const string InvalidCredentials = "Auth.InvalidCredentials";
            public const string UserNotFound = "Auth.UserNotFound";
            public const string PasswordIncorrect = "Auth.PasswordIncorrect";
            public const string TokenExpired = "Auth.TokenExpired";
            public const string TokenInvalid = "Auth.TokenInvalid";
            public const string TokenMissing = "Auth.TokenMissing";
            public const string UserAlreadyExists = "Auth.UserAlreadyExists";
            public const string UserCreated = "Auth.UserCreated";
            public const string UserUpdated = "Auth.UserUpdated";
            public const string UserDeleted = "Auth.UserDeleted";
        }
        public static class Patients
        {
            public const string NotFound = "Patients.NotFound";
            public const string Created = "Patients.Created";
            public const string Updated = "Patients.Updated";
            public const string Deleted = "Patients.Deleted";
            public const string NameRequired = "Patients.NameRequired";
            public const string EmailRequired = "Patients.EmailRequired";
            public const string InvalidEmail = "Patients.InvalidEmail";
            public const string PhoneRequired = "Patients.PhoneRequired";
            public const string InvalidPhone = "Patients.InvalidPhone";
            public const string CreatingFailed = "Patients.CreatingFailed";
        }

        public static class Treatments
        {
            public const string NotFound = "Treatments.NotFound";
            public const string Created = "Treatments.Created";
            public const string Updated = "Treatments.Updated";
            public const string Deleted = "Treatments.Deleted";
            public const string NameRequired = "Treatments.NameRequired";
        }

        public static class CarouselItems
        {
            public const string NotFound = "CarouselItems.NotFound";
            public const string Created = "CarouselItems.Created";
            public const string Updated = "CarouselItems.Updated";
            public const string Deleted = "CarouselItems.Deleted";
            public const string TitleRequired = "CarouselItems.TitleRequired";
            public const string ImageUrlRequired = "CarouselItems.ImageUrlRequired";
            public const string InvalidDisplayOrder = "CarouselItems.InvalidDisplayOrder";
            public const string Reactivated = "CarouselItems.Reactivated";
        }
    }
}
