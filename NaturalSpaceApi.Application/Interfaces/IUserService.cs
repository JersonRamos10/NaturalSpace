using NaturalSpaceApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetUserProfile (Guid userId); //obter el perfil del usuario por su ID

        void UpdateUser (Guid userId, UpdateUserRequest updateUser); //actualizar el perfil del usuario por su ID

        Task<IEnumerable<UserResponse>> GetUserWorkSpaceAsync(Guid workspaceId);  //obtener los usuarios que pertenecen a un espacio de trabajo específico por su ID
    }
}
