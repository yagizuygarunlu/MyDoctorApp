namespace WebApi.Common.Localization
{
    public static class LocalizationKeys
    {
        public static class Common
        {
            public const string Success = "Common.Success";
            public const string Error = "Common.Error";
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
        }
    }
}
