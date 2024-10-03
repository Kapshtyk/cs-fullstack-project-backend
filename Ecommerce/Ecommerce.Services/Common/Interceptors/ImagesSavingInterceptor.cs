using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ecommerce.Services.Common.Interceptors
{
    public class ImagesSavingInterceptor(IFileService fileService) : SaveChangesInterceptor
    {
        private readonly IFileService _fileService = fileService;
        private readonly List<string> _oldCategoryImages = [];
        private readonly List<string> _oldUserAvatars = [];

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var categoryEntries = context.ChangeTracker.Entries<Category>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in categoryEntries)
            {
                var oldImagePath = entry.OriginalValues.GetValue<string>("CategoryImage");
                var newImagePath = entry.CurrentValues.GetValue<string>("CategoryImage");

                if (oldImagePath != newImagePath)
                {
                    _oldCategoryImages.Add(oldImagePath);
                }
            }

            var userEntries = context.ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in userEntries)
            {
                var oldAvatarPath = entry.OriginalValues.GetValue<string>("Avatar");
                var newAvatarPath = entry.CurrentValues.GetValue<string>("Avatar");

                if (oldAvatarPath != newAvatarPath)
                {
                    _oldUserAvatars.Add(oldAvatarPath);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            foreach (var imagePath in _oldCategoryImages)
            {
                await _fileService.DeleteFileAsync(imagePath);
            }

            foreach (var avatarPath in _oldUserAvatars)
            {
                await _fileService.DeleteFileAsync(avatarPath);
            }

            _oldCategoryImages.Clear();
            _oldUserAvatars.Clear();

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            _oldCategoryImages.Clear();
            _oldUserAvatars.Clear();

            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }
    }
}
