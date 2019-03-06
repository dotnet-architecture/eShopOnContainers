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

Write-Host "setting username [$KIALIUSERNAME] and password [$KIALIPASSWORD]" -ForegroundColor Blue
kubectl apply -f secrets.yml