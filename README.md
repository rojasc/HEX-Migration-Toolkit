# HEX Migration Toolkit
The Hosted Exchange (HEX) Migration Toolkit is an open source project that provides
partners with a mechanism to in power their customers to perform various migration
operations. With this project customers will be able to configure migration endpoints, 
migration batches, and when they would like for the migration to start. All of this is 
accomplished utilizing the built-in tools to Microsoft Exchange 2013/2016 and PowerShell 
Remoting.

This project is comprised of two distinct components a website and an Azure WebJob. The website
utilizes Azure AD for authentication, and authorization is handled through claims that are injected 
by calls to the Microsoft Graph and Partner Center APIs when the user is authenticating. This means
the authenticating user must belong to a customer that has a relationship with the configured partner.
The Azure WebJob performs all the PowerShell remoting operations, and it can be easily scaled to 
handle a large volume of requests.

This project is being provided with community support only. If you need help please
log an issue using the [issue tracker](https://github.com/Microsoft/HEX-Migration-Toolkit/issues).

__Current Build Status:__ ![Build Status](https://ustechsales.visualstudio.com/_apis/public/build/definitions/08b6a9c4-c5bc-47c3-b945-aa13e7567100/20/badge)

## Deployment
Please follow the [deployment guide](docs/Deployment.md) to obtain the necessary information for the ARM template. 
Once all of the prerequisites have been fufilled please click the *Deploy to Azure* button below to deploy the solution.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FHEX-Migration-Toolkit%2Fmaster%2Fazuredeploy.json)
[![Visualize](http://armviz.io/visualizebutton.png)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FHEX-Migration-Toolkit%2Fmaster%2Fazuredeploy.json)

## Code of Conduct 
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more 
information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact 
[opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License
Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE) License.