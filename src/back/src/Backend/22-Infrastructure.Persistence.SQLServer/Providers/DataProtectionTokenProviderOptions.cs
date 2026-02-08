namespace Infrastructure.Persistence.SQLServer.Providers;

public class DataProtectionTokenProviderOptions
{
    /// <summary>
    /// Gets or sets the name of the <see cref="DataProtectorTokenProvider{TUser}"/>. Defaults to DataProtectorTokenProvider.
    /// </summary>
    /// <value>
    /// The name of the <see cref="DataProtectorTokenProvider{TUser}"/>.
    /// </value>
    public string Name { get; set; } = "DataProtectorTokenProvider";

    /// <summary>
    /// Gets or sets the amount of time a generated token remains valid. Defaults to 1 day.
    /// </summary>
    /// <value>
    /// The amount of time a generated token remains valid.
    /// </value>
    public TimeSpan TokenLifespan { get; set; } = TimeSpan.FromDays(1);
}
