# Description

Azure HTTP function app that provides data related to Ferguson Distribution Centers and Branches, such as branch number, address, city, state, zip latitude, longitude, and cutoff times.



# OpenAPI Definition

https://location-microservices.azurewebsites.net/api/Swagger/ui?code=ryGlh55fdNOW/8Q0EZbu3CsmgXKPzgWeimTuAWGuwdHYkJYxWk9nlA==



# CI/CD workflow and Pull Request instructions

1. Create a new branch with a name suitable for the code being added/refactored.

2. Send initial pull request to the **test** branch. Once merged, a build & deploy action in **Debug** configuration will be triggered to the _item-microservices-test_ function app. OpenAPI defintion here: https://location-microservices-test.azurewebsites.net/api/Swagger/ui?code=4XYHX/alngd4UBTLwKSASpS6ze9yayVWaxWzD9Cg360rvacXSRPGxQ==

3. Once approved in test, the next pull request should be sent to the **staging** branch. Once merged, a build & deploy action in **Release** configuration will be triggered to the _item-microservices_ staging environment (a deployment slot in the production function app). OpenAPI defintion here: https://location-microservices-staging.azurewebsites.net/api/Swagger/ui?code=fvDRj8Q3GNz/UBXQdfIz7EawTHiwuIQhOKFLhavAdl9r7Oo4TaufEQ==

4. Once approved in staging, the final pull request should be sent to the **master** branch. Once merged, a build & deploy action in **Release** configuration will be triggered to the _item-microservices_ production environment. OpenAPI defintion in section above.