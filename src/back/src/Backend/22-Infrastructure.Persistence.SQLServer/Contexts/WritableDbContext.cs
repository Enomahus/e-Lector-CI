using Infrastructure.Persistence.Attributes;
using Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.SQLServer.Contexts;

public class WritableDbContext : ApplicationDbContext
{
    private readonly TimeProvider _timeProvider;
    public WritableDbContext() { }

    public WritableDbContext(DbContextOptions<ApplicationDbContext> options, TimeProvider timeProvider)
        : base(options) 
    {
        _timeProvider = timeProvider;
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        var translationIdsToDelete = GetTranslationIdsToDelete();

        if (Database.IsRelational())
        {
            int result = 0;
            if(Database.CurrentTransaction == null)
            {
                var strategy = Database.CreateExecutionStrategy();
                await strategy.ExecuteInTransactionAsync(
                        async () =>
                        {
                            result = await SaveChangesAndDeleteTranslationsAsync(translationIdsToDelete, cancellationToken);
                        },
                        () => Task.FromResult(true)
                    );
            }
            else
            {
                result = await SaveChangesAndDeleteTranslationsAsync(translationIdsToDelete, cancellationToken );
            }
            return result;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> SaveChangesAndDeleteTranslationsAsync(HashSet<long> translationIdsToDelete, CancellationToken cancellationToken)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        //await Translations
        //    .Where(t => translationIdsToDelete.Contains(t))
        //    .ExecuteDeleteAsync(cancellationToken);

        return result;
    }

    private HashSet<long> GetTranslationIdsToDelete()
    {
        var deletedEntities = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        var translationIdsToDelete = new HashSet<long>();

        foreach (var entity in deletedEntities)
        {
            var properties = entity
                .GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(TranslationIdAttribute), false).Length != 0);
            foreach (var property in properties)
            {
                var obj = property.GetValue(entity);
                if (obj is not null)
                {
                    var translationId = (long)obj;
                    translationIdsToDelete.Add(translationId);
                }
            }
        }
        return translationIdsToDelete;
    }

    private void UpdateTimestamps()
    {
        var now = _timeProvider.GetUtcNow();

        // Optimisation de performance : On matérialise une fois les entrées modifiées
        // pour éviter de scanner le ChangeTracker de manière répétée.
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToDictionary(e => e.Entity, e => e.State);

        var timestampedEntries = ChangeTracker.Entries<ITimestampedEntity>().ToList();
        foreach (var entry in timestampedEntries)
        {
            bool hasDirectChanges = entry.State is EntityState.Added or EntityState.Modified;

            // On vérifie si les propriétés de navigation ont des changements
            bool hasNavigationChanges = HasChangedNavigationsRecursive(entry, modifiedEntries, new HashSet<object>());

            if(hasDirectChanges || hasNavigationChanges)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }

                entry.Entity.ModifiedAt = now;
            }

            //if (entry.State == EntityState.Added)
            //{
            //    entry.Entity.CreatedAt = now;
            //}
            //if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            //{
            //    entry.Entity.ModifiedAt = now;
            //}
            //UpdateTimestampByNavigationProperties(entry);
        }
    }

    private bool HasChangedNavigationsRecursive(EntityEntry entry, Dictionary<object, EntityState> modifiedEntries, HashSet<object> visited)
    {
        // Éviter les boucles infinies dans les relations circulaires
        if (!visited.Add(entry.Entity)) return false;

        foreach (var navigation in entry.Navigations)
        {
            if (navigation.CurrentValue == null) continue;

            // Cas 1 : La navigation est une collection
            if (navigation.CurrentValue is IEnumerable<object> collection)
            {
                foreach (var item in collection)
                {
                    if (IsModifiedOrDeepModified(item, modifiedEntries, visited))
                        return true;
                }
            }
            // Cas 2 : La navigation est une référence simple
            else
            {
                if (IsModifiedOrDeepModified(navigation.CurrentValue, modifiedEntries, visited))
                    return true;
            }
        }

        return false;
    }

    private bool IsModifiedOrDeepModified(object entity,Dictionary<object, EntityState> modifiedEntries, HashSet<object> visited)
    {
        // Si l'entité elle-même est marquée comme modifiée/ajoutée/supprimée
        if (modifiedEntries.ContainsKey(entity))
            return true;

        // Sinon, on plonge de manière récursive si c'est une entité suivie
        var entry = ChangeTracker.Entries().FirstOrDefault(e => e.Entity == entity);
        return entry != null && HasChangedNavigationsRecursive(entry, modifiedEntries, visited);
    }

    private void UpdateTimestampByNavigationProperties(EntityEntry<ITimestampedEntity> entry)
    {
        bool isModified = false;

        // Loop on navigation properties to detect changes.
        foreach (var currentItem in entry.Navigations.Select(n => n.CurrentValue))
        {
            // If the navigation property is a collection
            if (currentItem is IEnumerable<object> collection)
            {
                var itemsModified = collection.Where(IsItemModified).ToList();

                if (itemsModified.Count == 0)
                {
                    entry.Entity.ModifiedAt = _timeProvider.GetUtcNow();
                    isModified = true;
                }
            }
            else if (currentItem != null && IsItemModified(currentItem))
            {
                entry.Entity.ModifiedAt = _timeProvider.GetUtcNow();
                isModified = true;
            }

            if (isModified)
                break;
        }
    }

    private bool IsItemModified(object item)
    {
        return ChangeTracker.Entries().Any(e => e.Entity == item && e.State == EntityState.Modified);
    }
}
