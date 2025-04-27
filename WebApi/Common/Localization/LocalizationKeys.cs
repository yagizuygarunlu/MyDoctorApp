namespace WebApi.Common.Localization
{
    public static class LocalizationKeys
    {
        public static class Common
        {
            public const string Success = "Common.Success";
            public const string Error = "Common.Error";
            public const string NotFound = "Common.NotFound";
            public const string InvalidRequest = "Common.InvalidRequest";
            public const string Unauthorized = "Common.Unauthorized";
            public const string ServerError = "Common.ServerError";
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
            public const string Reactivated = "Treatments.Reactivated";
            public const string NameRequired = "Treatments.NameRequired";
            public const string QuestionRequired = "Treatments.QuestionRequired";
            public const string AnswerRequired = "Treatments.AnswerRequired";
            public const string TreatmentIdRequired = "Treatments.TreatmentIdRequired";
            public const string InvalidTreatmentId = "Treatments.InvalidTreatmentId";
            public const string CreationFailed = "Treatments.CreationFailed";
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

        public static class Doctors
        {
            public const string FullNameRequired = "Doctors.FullNameRequired";
            public const string FullNameMaxLength = "Doctors.FullNameMaxLength";
            public const string SpecialityRequired = "Doctors.SpecialityRequired";
            public const string SpecialityMaxLength = "Doctors.SpecialityMaxLength";
            public const string SummaryInfoRequired = "Doctors.SummaryInfoRequired";
            public const string SummaryInfoMaxLength = "Doctors.SummaryInfoMaxLength";
            public const string BiographyRequired = "Doctors.BiographyRequired";
            public const string BiographyMaxLength = "Doctors.BiographyMaxLength";
            public const string EmailRequired = "Doctors.EmailRequired";
            public const string InvalidEmail = "Doctors.InvalidEmail";
            public const string PhoneNumberRequired = "Doctors.PhoneNumberRequired";
            public const string InvalidPhoneNumber = "Doctors.InvalidPhoneNumber";
            public const string ImageUrlRequired = "Doctors.ImageUrlRequired";
            public const string InvalidImageUrl = "Doctors.InvalidImageUrl";
            public const string NotFound = "Doctors.NotFound";
            public const string Created = "Doctors.Created";
            public const string Updated = "Doctors.Updated";
            public const string Deleted = "Doctors.Deleted";
            public const string Reactivated = "Doctors.Reactivated";
            public const string InvalidId = "Doctors.InvalidId";
        }
        public static class Reviews
        {
            public const string DoctorIdRequired = "Reviews.DoctorIdRequired";
            public const string NameRequired = "Reviews.NameRequired";
            public const string MessageRequired = "Reviews.MessageRequired";
            public const string InvalidRating = "Reviews.InvalidRating";
            public const string Created = "Reviews.Created";
            public const string Updated = "Reviews.Updated";
            public const string Deleted = "Reviews.Deleted";
            public const string CreationFailed = "Reviews.CreationFailed";
        }
        public static class TreatmentFaqs
        {
            public const string QuestionRequired = "TreatmentFaqs.QuestionRequired";
            public const string AnswerRequired = "TreatmentFaqs.AnswerRequired";
            public const string TreatmentIdRequired = "TreatmentFaqs.TreatmentIdRequired";
            public const string InvalidTreatmentId = "TreatmentFaqs.InvalidTreatmentId";
            public const string Created = "TreatmentFaqs.Created";
            public const string CreationFailed = "TreatmentFaqs.CreationFailed";
            public const string NotFound = "TreatmentFaqs.NotFound";
            public const string Deleted = "TreatmentFaqs.Deleted";
            public const string Updated = "TreatmentFaqs.Updated";
        }
        public static class Exceptions
        {
            public const string InvalidArgument = "Exceptions.InvalidArgument";
            public const string ValidationError = "Exceptions.ValidationError";
            public const string ResourceNotFound = "Exceptions.ResourceNotFound";
            public const string AuthorizationFailed = "Exceptions.AuthorizationFailed";
            public const string InvalidOperation = "Exceptions.InvalidOperation";
            public const string NotImplemented = "Exceptions.NotImplemented";
            public const string UnexpectedError = "Exceptions.UnexpectedError";
            public const string TooManyRequests = "Exceptions.TooManyRequests";
        }
    }
}
