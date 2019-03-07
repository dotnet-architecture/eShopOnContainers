Param([parameter(Mandatory,HelpMessage="Enter a valid username for Kiali Administration")][string]$username,
      [parameter(Mandatory,HelpMessage="Enter your super secret password")][securestring]$password,
      [parameter(Mandatory=$false)][string]$NAMESPACE="istio-system"
)

function Get-PlainText()
{
	[CmdletBinding()]
	param
	(
		[parameter(Mandatory = $true)]
		[securestring]$SecureString
	)
	BEGIN { }
	PROCESS
	{
		$bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureString);
 
		try
		{
			return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr);
		}
		finally
		{
			[Runtime.InteropServices.Marshal]::FreeBSTR($bstr);
		}
	}
	END { }
}

$KIALIUSERNAME = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($username))
$plainpassword = Get-PlainText $password;
$KIALIPASSWORD = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($plainpassword))

<<<<<<< HEAD
Write-Host "Creating Kiali Secret in namespace [$NAMESPACE]" -ForegroundColor Blue
kubectl -n $NAMESPACE create secret generic kiali --from-literal=username=$KIALIUSERNAME --from-literal=passphrase=$KIALIPASSWORD
=======
Write-Host "setting username [$KIALIUSERNAME] and password [$KIALIPASSWORD]" -ForegroundColor Blue
kubectl apply -f secrets.yml
>>>>>>> abb108e03dd0ddf0b43d7ea4b2f5b307452ac88e
