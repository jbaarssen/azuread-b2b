#####
# Login to Azure and select the right subscription
#####
Login-AzureRmAccount -SubscriptionId 'YourSubscriptionId'

#####
# Create new key vault and set permissions to the owner
#####
New-AzureRmKeyVault -VaultName 'YourKeyVault' -ResourceGroupName 'YourResourceGroupName' -Location 'westeurope'
Set-AzureRmKeyVaultAccessPolicy -VaultName "RubiChillKeyVault" –UserPrincipalName "your@domain.com" –PermissionsToKeys all –PermissionsToSecrets all
