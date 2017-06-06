# Requires PowerShell to be run as Admin-level user.

New-SelfSignedCertificate -CertStoreLocation cert:\localmachine\my -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider" `
                          -Subject "cn=yourdomain.app.keyvault" -KeyDescription "Used to access Key Vault" `
                          -NotBefore (Get-Date).AddDays(-1) -NotAfter (Get-Date).AddYears(2)

#   PSParentPath: Microsoft.PowerShell.Security\Certificate::LocalMachine\my
#
#Thumbprint                                Subject
#----------                                -------
# FBE2B6190DF1C0B452756D6293E8418F4E33B32E  CN=yourdomain.app.keyvault
#

$pwd = ConvertTo-SecureString -String "W3lcome@YourPassword!" -Force -AsPlainText

# Export cert to PFX - uploaded to Azure App Service

Export-PfxCertificate -cert cert:\localMachine\my\FBE2B6190DF1C0B452756D6293E8418F4E33B32E `
                      -FilePath C:\Temp\Certificates\keyvaultaccess03.pfx -Password $pwd

#    Directory: C:\WINDOWS\system32
#
#Mode                LastWriteTime         Length Name
#----                -------------         ------ ----
#-a----       14/11/2016     16:06           2565 keyvaultaccess03.pfx
#

# Export Certificate to import into the Service Principal
Export-Certificate -Cert cert:\localMachine\my\FBE2B6190DF1C0B452756D6293E8418F4E33B32E `
                   -FilePath C:\Temp\Certificates\keyvaultaccess03.crt
