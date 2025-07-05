using System.Collections.Immutable;
using backend.src.Controllers;
using backend.src.Models;
using backend.src.Queries;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Extensions;

namespace backend.src.Services;

public enum Role
{
    Voter,
    BoardPresident,
    BoardVocal,
    BoardSecretary,
    Admin,
    Police,
}

public class SafeAuthorizeAttribute : AuthorizeAttribute
{
    public SafeAuthorizeAttribute(Role[] roles)
        : base()
    {
        Roles = RolesToString(roles);
    }

    public static string RolesToString(IEnumerable<Role> roles) =>
        string.Join(",", roles.Select(r => r.GetDisplayName()).ToImmutableSortedSet());
}

public interface IAuthService
{
    Task<AuthResponse<FullCitizen>> Login(LoginCredentials citizen);
}

public class AuthService : IAuthService
{
    public async Task<AuthResponse<FullCitizen>> Login(LoginCredentials citizen)
    {
        var select = await DB.Queries.LoginCitizen(
            new()
            {
                CredencialCivica = citizen.CredencialCivica,
                UruguayanId = citizen.UruguayanId,
            }
        ) ?? throw new InvalidOperationException("Citizen not found.");

        if (!Argon2.Verify(select.PasswordHash, citizen.Password)) {
            throw new InvalidOperationException("Wrong password.");
        }

        var rolesMap = new Dictionary<Role, bool>
        {
            [Role.Admin] = true, // TODO
            [Role.Voter] = select.CitizenId is not null, // TODO
            [Role.BoardPresident] = select.PollingStationPresidentId is not null,
        };

        var roles = rolesMap.Where(kv => kv.Value).Select(kv => kv.Key).ToImmutableList();

        return new AuthResponse<FullCitizen>
        {
            Circuit = new()
            {
                Building = new()
                {
                    Address = select.Address,
                    BuildingId = new Ulid(select.EstablishmentId),
                    Name = select.EstablishmentName,
                    Zone = new()
                    {
                        Locality = new()
                        {
                            Department = new()
                            {
                                DepartmentId = new Ulid(select.DepartmentId),
                                Name = select.DepartmentName,
                            },
                            LocalityId = new Ulid(select.LocalityId),
                            Type = select.Type switch
                            {
                                Queries.Codegen.LocalityType.City => LocalityType.City,
                                Queries.Codegen.LocalityType.Town => LocalityType.Town,
                                Queries.Codegen.LocalityType.Other => LocalityType.Other,
                                Queries.Codegen.LocalityType.Invalid => throw new InvalidOperationException(),
                            },
                        },
                        ZoneId = new Ulid(select.ZoneId),
                    }

                },
                CircuitId = new()
                {
                    CircuitNumber = select.PollingDistrictNumber,
                    EstablishmentId = new Ulid(select.EstablishmentId),
                }
            },
            JwtToken = "", // Placeholder
            Roles = roles,
            User = new FullCitizen
            {
                CitizenId = new Ulid(select.CitizenId),
                BirthDate = DateOnly.FromDateTime(select.Birth),
                CredencialCivica = select.CredencialCivica,
                Name = select.Name,
                Surname = select.Surname,
                UruguayanId = select.UruguayanId,
            },
        };
    }
}

