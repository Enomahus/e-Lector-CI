using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Infrastructure.Persistence.SQLServer.Providers;

public class DataProtectorTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser>
    where TUser : class
{

    /// <summary>
    /// Gets the <see cref="DataProtectionTokenProviderOptions"/> for this instance.
    /// </summary>
    /// <value>
    /// The <see cref="DataProtectionTokenProviderOptions"/> for this instance.
    /// </value>
    protected DataProtectionTokenProviderOptions Options { get; private set; }

    /// <summary>
    /// Gets the <see cref="IDataProtector"/> for this instance.
    /// </summary>
    /// <value>
    /// The <see cref="IDataProtector"/> for this instance.
    /// </value>
    protected IDataProtector Protector { get; private set; }

    /// <summary>
    /// Gets the name of this instance.
    /// </summary>
    /// <value>
    /// The name of this instance.
    /// </value>
    public string Name
    {
        get { return Options.Name; }
    }

    /// <summary>
    /// Gets the <see cref="ILogger"/> used to log messages from the provider.
    /// </summary>
    /// <value>
    /// The <see cref="ILogger"/> used to log messages from the provider.
    /// </value>
    public ILogger<DataProtectorTokenProvider<TUser>> Logger { get; }

    public DataProtectorTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger
    )
    {
        if (dataProtectionProvider == null)
        {
            throw new ArgumentNullException(nameof(dataProtectionProvider));
        }

        Options = options?.Value ?? new DataProtectionTokenProviderOptions();

        // Use the Name as the purpose which should usually be distinct from others
        Protector = dataProtectionProvider.CreateProtector(Name ?? "DataProtectorTokenProvider");
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(false);
    }

    public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        var ms = new MemoryStream();
        var userId = await manager.GetUserIdAsync(user);
        using (var writer = ms.CreateWriter())
        {
            writer.Write(DateTimeOffset.UtcNow);
            writer.Write(userId);
            writer.Write(purpose ?? "");
            string? stamp = null;
            if (manager.SupportsUserSecurityStamp)
            {
                stamp = await manager.GetSecurityStampAsync(user);
            }
            writer.Write(stamp ?? "");
        }
        var protectedBytes = Protector.Protect(ms.ToArray());
        return Convert.ToBase64String(protectedBytes);
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        try
        {
            var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using (var reader = ms.CreateReader())
            {
                var creationTime = reader.ReadDateTimeOffset();
                var expirationTime = creationTime + Options.TokenLifespan;
                if (expirationTime < DateTimeOffset.UtcNow)
                {
                    Logger.LogError("Token expired");
                    return false;
                }

                var userId = reader.ReadString();
                var actualUserId = await manager.GetUserIdAsync(user);
                if (userId != actualUserId)
                {
                    Logger.LogError("User doesn't match");
                    return false;
                }

                var purp = reader.ReadString();
                if (!string.Equals(purp, purpose))
                {
                    Logger.LogError("Purpose doesn't match");
                    return false;
                }

                var stamp = reader.ReadString();
                if (reader.PeekChar() != -1)
                {
                    Logger.LogError("Unexpected end of input");
                    return false;
                }

                if (manager.SupportsUserSecurityStamp)
                {
                    var isEqualsSecurityStamp = stamp == await manager.GetSecurityStampAsync(user);
                    if (!isEqualsSecurityStamp)
                    {
                        Logger.LogError("SecurityStamp not equal");
                    }

                    return isEqualsSecurityStamp;
                }

                var stampIsEmpty = stamp == "";
                if (!stampIsEmpty)
                {
                    Logger.LogError("SecurityStamp not empty");
                }

                return stampIsEmpty;
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            // Do not leak exception
            Logger.LogError("Unhandled exception");
        }

        return false;
    }
}


/// <summary>
/// Utility extensions to streams
/// </summary>
internal static class StreamExtensions
{
    internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

    public static BinaryReader CreateReader(this Stream stream)
    {
        return new BinaryReader(stream, DefaultEncoding, true);
    }

    public static BinaryWriter CreateWriter(this Stream stream)
    {
        return new BinaryWriter(stream, DefaultEncoding, true);
    }

    public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
    {
        return new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
    }

    public static void Write(this BinaryWriter writer, DateTimeOffset value)
    {
        writer.Write(value.UtcTicks);
    }
}