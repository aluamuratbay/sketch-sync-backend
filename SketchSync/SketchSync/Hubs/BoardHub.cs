using Microsoft.AspNetCore.SignalR;

namespace SketchSync.Hubs;

public class BoardHub : Hub
{
    public async Task UpdateShape(Guid boardId, string state)
    {
        await Clients.OthersInGroup(boardId.ToString())
            .SendAsync("Receive", state);
    }

    public async Task JoinBoard(Guid boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId.ToString());
    }
}