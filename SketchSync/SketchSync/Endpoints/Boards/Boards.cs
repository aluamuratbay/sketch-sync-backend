using SketchSync.Database.Repositories.Users;
using SketchSync.Database.Repositories.Boards;
using SketchSync.Endpoints.Boards.Contracts;
using SketchSync.Entities;
using SketchSync.Enums;
using SketchSync.Exceptions;
using SketchSync.Extensions;

namespace SketchSync.Endpoints.Boards;

public class Boards() : BaseModule(nameof(Boards))
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty).RequireAuthorization();

        group.MapGet("shared",
            async (IBoardRepository boardRepository, IUserRepository userRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var boards = await boardRepository.GetSharedAsync(userId);

                var memberUserIds = boards.SelectMany(b => b.Members.Select(m => m.UserId));
                var ownerUserIds = boards.Select(b => b.OwnerId);
                var allUserIds = memberUserIds
                    .Concat(ownerUserIds)
                    .Distinct()
                    .ToArray();
                var usersDict = (await userRepository.GetAsync(allUserIds))
                    .ToDictionary(u => u.Id);

                var boardsResponse = boards.Select(board =>
                {
                    var members = board.Members
                        .Where(m => m.UserId != userId)
                        .Select(m => BoardMemberResponse.FromUser(usersDict[m.UserId], m.Role))
                        .ToArray();

                    var role = board.Members.First(m => m.UserId == userId).Role;
                    var owner = BoardOwnerResponse.FromUser(usersDict[board.OwnerId]);

                    return SharedBoardResponse.FromBoard(board, role, owner, members);
                });

                return Results.Ok(boardsResponse);
            })
            .Produces<IEnumerable<SharedBoardResponse>>();
        
        group.MapGet("owned", async (IBoardRepository boardRepository, IUserRepository userRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var boards = await boardRepository.GetOwnedAsync(userId);
                
                var allUserIds = boards.SelectMany(b => b.Members.Select(m => m.UserId))
                    .Distinct()
                    .ToArray();
                
                var usersDict = (await userRepository.GetAsync(allUserIds))
                    .ToDictionary(u => u.Id);
    
               var boardsResponse = boards.Select(board =>
                    {
                        var members = board.Members
                            .Select(m => BoardMemberResponse.FromUser(usersDict[m.UserId], m.Role))
                            .ToArray();
    
                        return OwnedBoardResponse.FromBoard(board, members);
                    });
               
                return Results.Ok(boardsResponse);
            })
            .Produces<IEnumerable<OwnedBoardResponse>>();
        
        group.MapGet("shared/{boardId:guid}", async (Guid boardId, IBoardRepository boardRepository, IUserRepository userRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var board = await boardRepository.GetAsync(boardId);
                
                if (board.Members.All(u => u.UserId != userId)) 
                    throw new ForbiddenException($"User with ID {userId} is not a member of a board with ID {boardId}");

                var userIds = board.Members.Select(m => m.UserId).Concat([board.OwnerId]).ToArray();
                var usersDict = (await userRepository.GetAsync(userIds))
                    .ToDictionary(u => u.Id);
                
                var members = board.Members
                    .Where(m => m.UserId != userId)
                    .Select(m => BoardMemberResponse.FromUser(usersDict[m.UserId], m.Role))
                    .ToArray();

                var role = board.Members.First(m => m.UserId == userId).Role;
                var owner = BoardOwnerResponse.FromUser(usersDict[board.OwnerId]);

                var boardResponse = SharedBoardResponse.FromBoard(board, role, owner, members);
                return Results.Ok(boardResponse);
            })
            .Produces<SharedBoardResponse>();
        
        group.MapGet("owned/{boardId:guid}", async (Guid boardId, IBoardRepository boardRepository, IUserRepository userRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var board = await boardRepository.GetAsync(boardId);
                
                if (board.OwnerId != userId)
                    throw new ForbiddenException($"Forbidden resource for the user with the ID: {userId}");
                
                var userIds = board.Members.Select(m => m.UserId)
                    .ToArray();
                var usersDict = (await userRepository.GetAsync(userIds)).ToDictionary(u => u.Id);
                var members = board.Members
                    .Select(m => BoardMemberResponse.FromUser(usersDict[m.UserId], m.Role))
                    .ToArray();

                var boardResponse = OwnedBoardResponse.FromBoard(board, members);
                return Results.Ok(boardResponse);
            })
            .Produces<OwnedBoardResponse>();
        
        group.MapPost(string.Empty, async (CreateBoardRequest request, IBoardRepository boardRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var board = new Board
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    OwnerId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Content = string.Empty
                };
                await boardRepository.CreateAsync(board);
                return Results.Created();
            });

        group.MapPost("member", async (CreateMemberRequest request, IUserRepository userRepository, IBoardRepository boardRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
            
                var board = await boardRepository.GetAsync(request.BoardId);
    
                if (board.OwnerId != userId) 
                    throw new ForbiddenException($"Forbidden resource for the user with the ID: {userId}");
                
                var user = await userRepository.GetByEmailAsync(request.Email);
                
                if (user is null)
                    throw new NotFoundException($"User with email {request.Email} not found");

                if (userId == user.Id)
                    throw new BadRequestException($"User with email {request.Email} already exists");
                
                if(board.Members.Any(m => m.UserId == user.Id)) 
                    throw new BadRequestException($"User with ID {user.Id} already exists");
                
                var updatedMembers = board.Members.Append(new BoardMember
                {
                    UserId = user.Id,
                    Role = request.Role,
                });
                
                board.Members = updatedMembers.ToArray();
    
                await boardRepository.UpdateAsync(board);
                return Results.NoContent();
            });
        
        group.MapPut("member", async (UpdateMemberRequest request, IUserRepository userRepository, IBoardRepository boardRepository, HttpContext httpContext) =>
        {
            var userId = httpContext.GetUserId();
            
            if (userId == request.UserId)
                throw new BadRequestException($"User with email {request.UserId} already exists");
            
            var board = await boardRepository.GetAsync(request.BoardId);
    
            if (board.OwnerId != userId) 
                throw new ForbiddenException($"Forbidden resource for the user with the ID: {userId}");
            
            var member = board.Members.FirstOrDefault(m => m.UserId == request.UserId);
            
            if (member is null) 
                throw new BadRequestException($"User with ID {request.UserId} is not a member");
            
            member.Role = request.Role;
            await boardRepository.UpdateAsync(board);
            return Results.NoContent();
        });
        
        group.MapDelete("{boardId:guid}", async (Guid boardId, IBoardRepository boardRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var board = await boardRepository.GetAsync(boardId);
                if (board.OwnerId != userId)
                    throw new ForbiddenException($"Forbidden resource for the user with the ID: {userId}");
                
                await boardRepository.DeleteAsync(boardId);
                return Results.NoContent();
            });

        group.MapPut("{boardId:guid}", async (Guid boardId, UpdateBoardRequest request, IBoardRepository boardRepository, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var board = await boardRepository.GetAsync(boardId);

                if (board.OwnerId != userId)
                {
                    var member = board.Members.FirstOrDefault(m => m.UserId == userId);
                    if (member is null || member.Role == RoleEnum.Viewer)
                        throw new ForbiddenException($"Forbidden resource for the user with the ID: {userId}");
                }

                var updatedBoard = request.MapToBoard(board);
                await boardRepository.UpdateAsync(updatedBoard);
                return Results.NoContent();
            });
    }
}