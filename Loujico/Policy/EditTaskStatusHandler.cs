using Loujico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;

public class EditTaskStatusRequirement : IAuthorizationRequirement
{
}

public class EditTaskStatusHandler : AuthorizationHandler<EditTaskStatusRequirement, int>
{
    private readonly CompanySystemContext _ctx;

    public EditTaskStatusHandler(CompanySystemContext ctx)
    {
        _ctx = ctx;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EditTaskStatusRequirement requirement,
        int taskId)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Team_Leader"))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.IsInRole("Programmer"))
        {
            var employeeId = context.User.FindFirst("EmployeeId")?.Value;
            if (employeeId == null) return;

            bool isAssigned = await _ctx.TbProjectTaskEmployees
                .AnyAsync(te => te.TaskId == taskId && te.EmployeeId == int.Parse(employeeId) && !te.IsDeleted);

            if (isAssigned) context.Succeed(requirement);
        }
    }
}
