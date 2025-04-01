namespace WebApi.Domain.ValueObjects
{
    public record Address
    {
        public required string AddressLine { get; init; }
        public required string Street { get; init; }
        public required string District { get; init; }
        public required string Country { get; init; }
        public required string City { get; init; }
        public required string ZipCode { get; init; }

        public Address()
        {
        }
        public Address(string addressLine, string street, string district, string country, string city, string zipCode)
        {
            AddressLine = addressLine;
            Street = street;
            District = district;
            Country = country;
            City = city;
            ZipCode = zipCode;
        }
    }
}
