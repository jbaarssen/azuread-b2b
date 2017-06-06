# http://stackoverflow.com/questions/20852807/setting-private-key-permissions-with-powershell

param(
    [Parameter(Mandatory=$true)]
    [String]
    $CertThumbprint,
    
    [Parameter(Mandatory=$true)]
    [String]
    $AccountName
)

# Get certificate.
$cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object -FilterScript { $PSItem.Thumbprint -eq $CertThumbprint }

if ($cert) {

    Write-Output "Found certificate: $cert"
    
    # Location of private key file.
    $keyPath = $env:ProgramData + "\Microsoft\Crypto\RSA\MachineKeys\"
    $keyName = $cert.PrivateKey.CspKeyContainerInfo.UniqueKeyContainerName
    $keyFullPath = $keyPath + $keyName

    Write-Output "Key path: $keyFullPath"

    # Specify the user, the permissions and the permission type.
    $permission = "$($AccountName)","Read","Allow"
    $accessRule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $permission

    # Current ACL.
    $acl = Get-Acl -Path $keyFullPath

    # Add new access rule to ACL.
    $acl.AddAccessRule($accessRule)

    # Write back new ACL.
    Set-Acl -Path $keyFullPath -AclObject $acl
}
else {
    Write-Error -Message "Certificate not found"
}
