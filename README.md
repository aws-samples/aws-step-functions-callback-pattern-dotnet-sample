## Introduction
In workflow based application execution, few steps require a pause and wait for the confirmation/approval from external sources, after getting confirmation it resumes or terminates the execution. To accomplish this, a callback pattern can be used where an AWS Step function pause during a task, and wait for an external process or command to return the task token, that was generated when the task started. 
This sample provides implementation of callback pattern for approval or confirmation step from external API/service with the help of Amazon S3, and AWS Lambda functions.

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