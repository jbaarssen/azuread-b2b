#####
# Prepare Cert for use with Service Principal
#####

$x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$x509.Import("C:\Temp\Certificates\keyvaultaccess03.crt")
$credValue = [System.Convert]::ToBase64String($x509.GetRawCertData())
# should match our certificate entries above.
$validFrom = (Get-Date).AddDays(-1)
$validTo = [System.DateTime]::Parse("2019-05-09")

##
# Create new Service Principal with Cert configured
##

Login-AzureRmAccount -SubscriptionId 'YourSubscriptionId'

# $credValue comes from the previous script and contains the X509 cert we wish to use.
# $validFrom comes from the previous script and is the validity start date for the cert.
# $validTo comes from the previous script and is the validity end data for the cert.

$adapp = New-AzureRmADApplication -DisplayName "YourKeyVault KeyVault" -HomePage "https://yourkeyvault/" `
                                  -IdentifierUris "https://yourkeyvault/" -CertValue $credValue `
                                  -StartDate $validFrom -EndDate $validTo
#
# DisplayName             : KeyVault Reader - Cert
# ObjectId                : XXXXXXXX-XXXX-XXXX-XXXX-1029a4c5be13
# IdentifierUris          : {https://keyvaultreadr/}
# HomePage                : https://keyvaultreadr/
# Type                    : Application
# ApplicationId           : XXXXXXXX-XXXX-XXXX-XXXX-b1aa47a95554
# AvailableToOtherTenants : False
# AppPermissions          :
# ReplyUrls               : {}
#

New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId

# DisplayName                    Type                           ObjectId
# -----------                    ----                           --------
# KeyVault Reader - Cert         ServicePrincipal               XXXXXXXX-XXXX-XXXX-XXXX-11b962b59eef

####
# Grant Service Principal Read-Only on Secrets in our KeyVault
####

Set-AzureRmKeyVaultAccessPolicy -VaultName 'YourKeyVault' -ResourceGroupName 'YourResourceGroup' `
                                -ServicePrincipalName $adapp.ApplicationId.Guid `
                                -PermissionsToSecrets get

##
# Print Out the Service Principal's App ID (GUID) to use later in our Function setup.
##
$adapp.ApplicationId