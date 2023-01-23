## Introduction
In workflow based application execution, few steps require a pause and wait for the confirmation/approval from external sources, after getting confirmation it resumes or terminates the execution. To accomplish this, a callback pattern can be used where an AWS Step function pause during a task, and wait for an external process or command to return the task token, that was generated when the task started. 
This sample provides implementation of callback pattern for approval or confirmation step from external API/service with the help of Amazon S3, and AWS Lambda functions.
This is the architecture that this sample implements.
![callback-pattern-sample-architecture](https://gitlab.aws.dev/umaskr/aws-step-functions-callback-patterns-sample/-/tree/main/blob/callback-pattern.png?raw=true)

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
11. As it gets SendTaskSucess, it resumes the execution.
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
    `cdk synth`\
    `cdk deploy`
	
## Security

See [CONTRIBUTING](CONTRIBUTING.md#security-issue-notifications) for more information.

## License

This library is licensed under the MIT-0 License. See the LICENSE file.