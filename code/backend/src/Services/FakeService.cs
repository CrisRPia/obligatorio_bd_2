using backend.src.Models;
using Bogus;

namespace backend.src.Services;

public interface IFakeService
{
    FullCitizen FakeCitizen(int? uruguayanIdWithoutIdentifier = null);
    IEnumerable<FullCitizen> FakeCitizens(IEnumerable<int> uruguayanIdsWithoutVerifier);
    string GenerateUniqueCredencialCivica();
}

public class FakeService() : IFakeService
{
    public Faker Faker { get; init; } = new("es");

    public FullCitizen FakeCitizen(int? uruguayanIdWithoutIdentifier = null)
    {
        return FakeCitizens([uruguayanIdWithoutIdentifier ?? Faker.Random.Number(100_000_00)])
            .First();
    }

    public IEnumerable<FullCitizen> FakeCitizens(IEnumerable<int> uruguayanIdsWithoutVerifier)
    {
        return uruguayanIdsWithoutVerifier.Select(uid =>
        {
            var generatedUid = uid * 10 + UruguayanIdVerifier.GetValidationDigit(uid);

            return new FullCitizen
            {
                Name = Faker.Name.FirstName(),
                Surname = Faker.Name.LastName(),
                UruguayanId = generatedUid ?? throw new NotImplementedException(),
                CredencialCivica = GenerateUniqueCredencialCivica(),
                BirthDate = Faker.Date.PastDateOnly(
                    refDate: DateOnly.FromDateTime(
                        DateTime.Today - TimeSpan.FromDays(1) * 366 * 18
                    ),
                    yearsToGoBack: 80
                ),
            };
        });
    }

    private static long CredencialCivicaCounter = 10000;

    public string GenerateUniqueCredencialCivica()
    {
        return string.Join(
                "",
                [
                    Faker.Lorem.Letter(),
                    Faker.Lorem.Letter(),
                    Faker.Lorem.Letter(),
                    CredencialCivicaCounter++.ToString(),
                ]
            )
            .ToUpperInvariant();
    }
}
