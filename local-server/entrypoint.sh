#!/bin/bash

env
dotnet ./AzureDDNS.dll --azurewebhook=$AZURE_WEBHOOK --key=$KEY --subdomain=$SUBDOMAIN --ipcheckuri=$IP_CHECK_URI