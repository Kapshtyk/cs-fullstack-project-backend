using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.UserService.DTO;
using Ecommerce.Services.UserService.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.UserService
{
    public class UserService(
        IUserRepo userRepo,
        IHashingService hashingService,
        ISaltRepo saltRepo,
        IDistributedCache cache,
        IFileService fileService
        ) : BaseService<User, UserFilterOptions, GetUserDto, CreateUserDto, UpdateUserDto, PartialUpdateUserDto>(userRepo, cache),
        IUserService
    {
        private readonly IUserRepo _repo = userRepo;
        private readonly ISaltRepo _saltRepo = saltRepo;
        private readonly IHashingService _hashingService = hashingService;
        private readonly IFileService _fileService = fileService;

        public override async Task<GetUserDto> CreateAsync(CreateUserDto dto)
        {
            var salt = _hashingService.GenerateSalt();
            _hashingService.HashValue(dto.Password, out string hashedPassword, salt);
            dto.Password = hashedPassword;

            if (dto.Avatar != null)
            {
                var relativeFilePath = await _fileService.SaveFileAsync(dto.Avatar, "avatars");
                dto.AvatarPath = relativeFilePath;
            }

            var result = await base.CreateAsync(dto);
            var user = await _repo.GetByIdAsync(result.Id);

            await _saltRepo.SetSaltAsync(user, salt);

            return result;
        }
        public override async Task<GetUserDto> UpdateAsync(UpdateUserDto dto, int id)
        {
            var existingUser = await _repo.GetByIdAsync(id);
            var salt = await _saltRepo.GetSaltAsync(existingUser);

            _hashingService.HashValue(dto.Password, out string hashedPassword, salt);
            dto.Password = hashedPassword;

            if (dto.Avatar != null)
            {
                var relativeFilePath = await _fileService.SaveFileAsync(dto.Avatar, "avatars");
                dto.AvatarPath = relativeFilePath;
            }

            return await base.UpdateAsync(dto, id);
        }

        public override async Task<GetUserDto> UpdateAsync(PartialUpdateUserDto dto, int id)
        {
            var existingUser = await _repo.GetByIdAsync(id);

            if (dto.Password != null)
            {
                var salt = await _saltRepo.GetSaltAsync(existingUser);
                _hashingService.HashValue(dto.Password, out string hashedPassword, salt);
                dto.Password = hashedPassword;
            }

            if (dto.Avatar != null)
            {
                var relativeFilePath = await _fileService.SaveFileAsync(dto.Avatar, "avatars");
                dto.AvatarPath = relativeFilePath;
            }

            return await base.UpdateAsync(dto, id);
        }
    }
}