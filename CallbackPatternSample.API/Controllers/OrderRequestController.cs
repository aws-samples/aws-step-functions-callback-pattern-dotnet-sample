using Amazon.S3;
using Amazon.S3.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using CallbackPatternSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CallbackPatternSample.API.Controllers;

[Route("[controller]")]
[ApiController]
public class OrderRequestController : ControllerBase
{
#pragma warning disable CS8601 // Possible null reference assignment.
    private readonly string OrderStateMachineArn = Environment.GetEnvironmentVariable("OrdersStateMachine");
#pragma warning restore CS8601 // Possible null reference assignment.

    [HttpPost("OrderStatus/{OrderId}")]
    public async Task<IActionResult> GetOrderStatus(Guid OrderId)
    {
        await Task.Delay(100);
        return Ok();
    }

    [HttpPost("ProcessOrder")]
    public async Task<IActionResult> ProcessOrder([FromBody] Order order)
    {
        using (var amazonStepFunctionsClient = new AmazonStepFunctionsClient())
        {
            // start the execution
            var startExecutionRequest = new StartExecutionRequest
            {
                Input = System.Text.Json.JsonSerializer.Serialize(order),
                StateMachineArn = OrderStateMachineArn,
                Name = order.OrderId.ToString()+Guid.NewGuid().ToString()
            };

            await amazonStepFunctionsClient.StartExecutionAsync(startExecutionRequest);
        }
        return new ObjectResult("Order request received and processing strated...") { StatusCode = (int)HttpStatusCode.OK };
    }

    [HttpPost("CompleteOrder")]
    public async Task<IActionResult> CompleteOrder([FromBody] Order order)
    {
        string? token = await GetTaskToken(order.OrderId);
        if (!string.IsNullOrEmpty(token))
        {
            using (var amazonStepFunctionsClient = new AmazonStepFunctionsClient())
            {
                if (order.IsConfirmed)
                {
                    var response = await amazonStepFunctionsClient.SendTaskSuccessAsync(new SendTaskSuccessRequest()
                    {
                        Output = System.Text.Json.JsonSerializer.Serialize(order),
                        TaskToken = token
                    });
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                        return new ObjectResult("Order Confirmed.") { StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    var response = await amazonStepFunctionsClient.SendTaskFailureAsync(new SendTaskFailureRequest()
                    {
                        Cause = "Cancelled from client...",
                        TaskToken = token
                    });
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                        return new ObjectResult("Order not confirmed..") { StatusCode = (int)HttpStatusCode.OK };
                }
            }
        }

        return new ObjectResult("Order not confirmed, contact support.") { StatusCode = (int)HttpStatusCode.InternalServerError };
    }    

    public static async Task<string> GetTaskToken(Guid orderId)
    {
        Console.WriteLine("Getting token for : "+ orderId.ToString());
        string? token = null;
        using (var s3Client = new AmazonS3Client())
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName =Environment.GetEnvironmentVariable("TokenStoreBucket");
            request.Key = orderId.ToString();
            var response = await s3Client.GetObjectAsync(request);
            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                token = reader.ReadToEnd();
            }
        }
        Console.WriteLine("token: " + token);
        return token;
    }



}