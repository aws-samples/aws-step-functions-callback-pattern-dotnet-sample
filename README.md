## Introduction
In workflow based application execution, few steps require a pause and wait for the confirmation/approval from external sources, after getting confirmation it resumes or terminates the execution. To accomplish this, a callback pattern can be used where an AWS Step function pause during a task, and wait for an external process or command to return the task token, that was generated when the task started. 
This sample provides implementation of callback pattern for approval or confirmation step from external API/service with the help of Amazon S3, and AWS Lambda functions.

# Projects in solution
**CallbackPatternSample.API**- API that trigger Step functions workflow execution and send confirmation request.\
**CallbackPatternSample.Models**- Shared library used across the projects.\
**cdk**- To create all the components of this sample using Amazon CDK toolkit.\
**completeOrderFunction**- A lambda function to mark order as completed.\
**processOrderFunction**- A lambda function to start processing of order.\
**storeTaskTokenFunction**- A lambda function to store task token into Amazon S3.

# Architecture
This is the architecture that this sample implements.
![callback-pattern-sample-architecture](https://github.com/aws-samples/aws-step-functions-callback-pattern-dotnet-sample/blob/main/blob/callback-pattern.png)

1. User sends process order request.
2. API Lambda function validate request.
3. API Lambda trigger the setp function workflow.
4. API Lambda sends acknowledgement response to the user.
5. Process order task execution starts.
6. Execution paused and wait for confirmation API, invoke storeTaskToken lambda function.
7. storeTaskToken lambda function stores task token into S3 bucket.
8. User sends confirmation request to API.
9. API lambda validate confirmation request.
10. API lambda fetch task token from S3 for given request and sends task sucess to step functions.
11. As it gets `SendTaskSucess`, it resumes the execution.
12. API lambda sends acknowledgement response to the user.
13. Starts complete order task\
\
***Note: AWS Batch, Fargate instance, Elastic Container Registry (ECR) are applicable if task in Step function task is taking more than 15 minutes and considering that task running into AWS Batch instead of lambda function.***

## Pre-requisites:
Visual Studio\
AWS Toolkit for Visual Studio\
.NET 6 or above

## Deployment

1. Set enviornment variable `CDK_DEFAULT_ACCOUNT` by your AWS account ID
2. Install aws-cdk [https://docs.aws.amazon.com/cdk/v2/guide/cli.html]
3. Build the solution
3. Open command prompt and run below commands\
    `dotnet publish -c Release`
    `cdk synth`\
    `cdk deploy`

## Testing

After deployment, you will get API Gateway endpoint URL under outputs. You can use that URL to create the requests using Postman/Swagger or any other tool for REST APIs.

1. Process Order Request:\
Method: POST\
URL: {Your API Gateway endpoint URL}/OrderRequest/ProcessOrder\
Headers: x-api-key: {Take the same value that is passed in cdkstack.cs}\
Body:
    `{
    "OrderId":"2d6bfae2-c279-41d5-b59e-280b22733f9d",
    "OrderDetails":"order1"
    }`
2. Check the workflow execution in AWS Console, it must be Paused and waiting for confirmation.
3. Confirm/Complete Order Request:\
Method: POST\
URL: {Your API Gateway endpoint URL}/OrderRequest/CompleteOrder\
Headers: x-api-key: {Take the same value that is passed in cdkstack.cs}\
Body:
    `{
    "OrderId":"2d6bfae2-c279-41d5-b59e-280b22733f9d",
    "OrderDetails":"order1",
    "IsConfirmed":true
    }`

## Security

See [CONTRIBUTING](CONTRIBUTING.md#security-issue-notifications) for more information.

## License

This library is licensed under the MIT-0 License. See the LICENSE file.