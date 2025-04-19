using Microsoft.Extensions.Localization;

namespace WebApi.Common.Localization
{
    public class LocalizationService: ILocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        public LocalizationService(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }
        public string GetLocalizedString(string key)
        {
            return _localizer[key];
        }
    }
}
