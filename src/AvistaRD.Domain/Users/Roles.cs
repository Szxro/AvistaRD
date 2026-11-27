namespace AvistaRD.Domain.Users;

public static class Roles
{
    public const string Administrator = "Administrador";

    public const string Validator = "Validador";

    public const string ParkRanger = "Guardaparques";

    public const string Citizen = "Ciudadano";

    public static readonly IReadOnlyList<string> All = [Administrator, Validator, ParkRanger, Citizen];

    // Roles someone can choose when creating an account; only an administrator can grant Administrador
    public static readonly IReadOnlyList<string> Registrable = [Citizen, Validator, ParkRanger];

    // Citizens can use the platform right away; the other roles wait for an administrator's approval
    public static bool RequiresApproval(string role) => role != Citizen;
}
