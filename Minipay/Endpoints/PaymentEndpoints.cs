using Microsoft.AspNetCore.Mvc;
using Minipay.Api.Contracts;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.AuthorizePayment;
using Minipay.Application.Payments.Command.CreatePayment;
using Minipay.Application.Payments.Command.FailPayment;
using Minipay.Application.Payments.Command.SettlePayment;
using Minipay.Application.Payments.Dtos;
using Minipay.Application.Payments.Exceptions;
using Minipay.Application.Payments.Queries.GetPaymentById;
using Minipay.Application.Payments.Queries.GetPaymentByStatus;
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;

namespace Minipay.Api.Endpoints;


public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/payments").WithTags("Payments");

        group.MapPost("/", CreatePaymentAsync)
            .WithName("CreatePayment")
            .WithSummary("Create a new payment")
            .Produces<CreatePaymentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPost("/{id:guid}/authorize", AuthorizePaymentAsync)
            .WithName("AuthorizePayment")
            .WithSummary("Authorize an existing payment")
            .Produces<PaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/{id:guid}", GetPaymentByIdAsync)
            .WithName("GetPaymentById")
            .WithSummary("Get a payment by its Id")
            .Produces<PaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetPaymentsByStatusAsync)
            .WithName("GetPaymentsByStatus")
            .WithSummary("List payments filtered by status, e.g. /payments?status=Authorized")
            .Produces<IReadOnlyCollection<PaymentResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPost("/{id:guid}/fail", FailPaymentAsync)
            .WithName("FailPayment")
            .WithSummary("Fail an existing payment")
            .Produces<PaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();

        group.MapPost("/{id:guid}/settle", SettlePaymentAsync)
            .WithName("SettlePayment")
            .WithSummary("Settle an existing payment")
            .Produces<PaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/statistics", GetStatisticsAsync)
            .WithName("GetStatistics")
            .WithSummary("Get all payment count since the app started")
            .Produces<PaymentStatisticsDto>(StatusCodes.Status200OK);
    }



    private static async Task<IResult> CreatePaymentAsync(
        [FromBody] CreatePaymentRequest request,
        [FromServices]CreatePaymentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await handler.HandleAsync(
                new CreatePaymentCommand(request.Amount, request.Currency), cancellationToken);

          
            return Results.Created($"/payments/{dto.Id}", new CreatePaymentResponse(dto.Id));
        }
        catch (ArgumentException ex)
        {
       
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [ex.ParamName ?? "request"] = new[] { ex.Message }
            });
        }
    }

    private static async Task<IResult> AuthorizePaymentAsync(
        Guid id,
        [FromServices] AuthorizePaymentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await handler.HandleAsync(new AuthorizePaymentCommand(id), cancellationToken);
            return Results.Ok(PaymentResponse.FromDto(dto));
        }
        catch (PaymentNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidPaymentStateTransitionException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetPaymentByIdAsync(
        Guid id,
        [FromServices] GetPaymentByIdHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await handler.HandleAsync(new GetPaymentByIdQuery(id), cancellationToken);
            return Results.Ok(PaymentResponse.FromDto(dto));
        }
        catch (PaymentNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetPaymentsByStatusAsync(
        [FromQuery] string? status,
       [FromServices] GetPaymentByStatusHandler handler,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(status) || !Enum.TryParse<PaymentStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["status"] = new[]
                {
                    $"'{status}' is not a valid payment status. Valid values: {string.Join(", ", Enum.GetNames<PaymentStatus>())}."
                }
            });
        }

        var payments = await handler.HandleAsync(new GetPaymentByStatusQuery(parsedStatus), cancellationToken);
        return Results.Ok(payments.Select(PaymentResponse.FromDto));
    }

    private static async Task<IResult> FailPaymentAsync(
        Guid Id,
        [FromBody] FailPaymentRequest Request,
        [FromServices] FailPaymentHandler handler,
        CancellationToken cancellationToken)

    {
        if (string.IsNullOrWhiteSpace(Request.Reason))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["reason"] = new[] { "A reason is required when failing a payment" }
            });
            
        }
        try
        {
            var dto = await handler.HandleAsync(new FailPaymentCommand(Id, Request.Reason), cancellationToken);
            return Results.Ok(PaymentResponse.FromDto(dto));
        }
        catch (PaymentNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidPaymentStateTransitionException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    }
    public static async Task<IResult> SettlePaymentAsync(
        Guid id,
        [FromServices] SettlePaymentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await handler.HandleAsync(new SettlePaymentCommand(id), cancellationToken);
            return Results.Ok(PaymentResponse.FromDto(dto));
        }
        catch (PaymentNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidPaymentStateTransitionException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    }
    private static IResult GetStatisticsAsync(
        [FromServices] IPaymentStatisticsService statisticsService)
    {
        return Results.Ok(statisticsService.GetStatistics());
    }
        
}

