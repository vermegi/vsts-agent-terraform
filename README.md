# vsts-agent-terraform

This is a sample project, showing how you can use a dockerized build agent, which you spin up and down when needed. 
All explanation about how this is set up and how this can be used is at:

* http://vermegi.github.io/azure/terraform/serverless/2018/08/14/deploying-a-serverless-app-on-azure-with-terraform-part1/
* http://vermegi.github.io/azure/terraform/serverless/2018/08/20/deploying-a-serverless-app-on-azure-with-terraform-part2/
* http://vermegi.github.io/azure/terraform/serverless/2018/08/29/deploying-a-serverless-app-on-azure-with-terraform-part3/
* http://vermegi.github.io/azure/terraform/serverless/2018/08/29/deploying-a-serverless-app-on-azure-with-terraform-addendum/ 

This repo contains the dockerfile for this build agent and the code of the function that starts and stops this container in Azure Container Instance. 

This code also comes with no guarantees, but still, enjoy :) 