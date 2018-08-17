FROM microsoft/vsts-agent:latest

# Build-time metadata as defined at http://label-schema.org
LABEL org.label-schema.name="VSTS Agent with Terraform Tools" \
    org.label-schema.url="https://github.com/vermegi/" \
    org.label-schema.vcs-url="https://github.com/vermegi/vsts-agent-terraform" \
    org.label-schema.schema-version="1.0"
                
ENV TERRAFORM_VERSION 0.11.8

# Install Terraform
RUN echo "===> Installing Terraform ${TERRAFORM_VERSION}..." \
 && wget https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip \
 &&	unzip terraform_${TERRAFORM_VERSION}_linux_amd64.zip \
 && mv terraform /usr/local/bin/terraform \
 && rm terraform_${TERRAFORM_VERSION}_linux_amd64.zip 
 