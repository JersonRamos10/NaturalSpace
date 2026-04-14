using FluentValidation;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Infrastructure.Data.Context;


using Mapster;

namespace NaturalSpaceApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly NaturalSpaceContext _context;

        private readonly IValidator<RegisterRequest> _validator;

        private readonly ITokenService _tokenService;

        public AuthService(
            NaturalSpaceContext context,
            IValidator<RegisterRequest> validator,
            ITokenService token)
        {
            _context = context;
            _validator = validator;
            _tokenService = token;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {

            //validar si el usuario existe por email o username
            var user = _context.Users.Where(u => u.Email == loginRequest.Email || u.UserName == loginRequest.UserName)
                                            .FirstOrDefault();
            if(user == null)
                throw new NotFoundException($"User not found, email or username incorrect");

            //verificar la contraseña
            bool  isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);

            //generar el token de autenticacion

            var token = _tokenService.GenerateToken(user);

            //retornar el token y la fecha de expiracion

            var response = new AuthResponse
            ( 
                Token: token,
                Expiration: DateTime.UtcNow.AddHours(1)
             
             ); 

            return  response;
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            //validar el request
            var validationResult = _validator.Validate(registerRequest);

            if (validationResult.IsValid == false)
            {
                throw new ValidationException(validationResult.Errors);
            }

            //validacion de negocio - verificar si el email o username ya existen 
            var emailExist = await _context.Users.AnyAsync(u => u.Email == registerRequest.Email);
                if (emailExist)
                  throw new ConflictException($"Email already in used");
           

            var userExist = await _context.Users.AnyAsync(u => u.UserName == registerRequest.UserName);

            if (userExist)
                throw new ConflictException($"Username [{registerRequest.UserName}] is already in Used");


            var user = createUser(registerRequest);//invocar el metodo privado para crear un nuevo usuario

            _context.Users.Add(user);    
            await _context.SaveChangesAsync();
            
         
            return user.Adapt<UserResponse>();

        }


        //metodos privados de la clase 

        //metodo privado para hashear la contraseña del usuario
        private string HashPassword(string password)
        {
           string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
           
            return hashedPassword;
        }

        //metodo privado para crear un nuevo usuario a partir del request de registro
        private User createUser(RegisterRequest request)
        {
            return new User {
                Name = request.Name,
                UserName = request.UserName,
                Email= request.Email,
                PasswordHash = HashPassword(request.Password),
                AvatarUrl = null,
                CreatedAt = DateTime.UtcNow
            
            };
        } 
    }
}
