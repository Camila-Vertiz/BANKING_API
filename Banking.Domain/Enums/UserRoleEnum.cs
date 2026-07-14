namespace Banking.Domain.Enums
{
    /// <summary>
    /// Roles disponibles dentro del sistema bancario.
    /// </summary>
    public enum UserRoleEnum
    {
        /// <summary>
        /// Usuario administrador con permisos administrativos.
        /// </summary>
        Admin = 1,
        /// <summary>
        /// Usuario cliente que gestiona sus propias cuentas.
        /// </summary>
        Customer = 2
    }
}
