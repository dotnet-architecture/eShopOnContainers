BDD.CORE.WEB
------------

- AzDevOps Sync
  - Use [Spex](https://vamsitp.github.io/spexdocs/)

- Code files
	- `Core\AppTestBase.cs`: Used for "Hooks"

- Configuration
	- `app.config`: Change values under `<spex>` node for Spex (AzDevOps-Sync)
	- Make sure the values of DefaultAssignedTo (in .config) / @owner tag (in .feature) are valid.
		- e.g.: The alias vamsitp(@microsoft.com) is different than vamsi.tp(@microsoft.com) - though both are valid aliases. AzDevOps only honors that one that was added to the account

- Main classes to use
    - `UIStepDefinitionBase`: To add additional functionality, inherit this class and add/override methods
    - `ProjectPageBase`: To add additional functionality, inherit this class and add/override methods
    - `ElementPage`: To add additional functionality, inherit this class and add/override methods
    - `UrlPage`: To add additional functionality, inherit this class and add/override methods
    - `WindowPage`: To add additional functionality, inherit this class and add/override methods
	- You can add more Pages / PageObjects as you deem fit for your project (see NOTE below)

- Tools
	- `muppet.cmd`: Used for Parallel-test-runs (Uses [jrepl.bat](https://www.dostips.com/forum/viewtopic.php?t=6044))

- Scenario specific Packages
    - `Bdd.Core.Web`: For Web Tests
    - `Bdd.Core.Api`: For Api Tests