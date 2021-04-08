// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppTestBase.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Bdd.Core.Hooks
{
    using System.Threading.Tasks;

    using Bdd.Core.Web.Hooks;

    using TechTalk.SpecFlow;

    /// <summary>
    /// https://github.com/techtalk/SpecFlow/wiki/Hooks#supported-hook-attributes
    /// </summary>
    [Binding]
    public class AppTestBase : WebProjectTestBase
    {
        private const int Order = 1;

        /// <summary>
        /// Assembly initialization code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeTestRun(Order = Order)]
        public static async Task AssemblyInitialize()
        {
            //// // If retrieval of AccessToken from AD isn't possible to connect to SQL DB, comment out the below line
            //// DataSources.SqlDataSource.AccessTokenCallback = SqlConnectionConfig.AccessTokenCallback;
            //// DataSources.SqlDataSource.ConnectionBuilderCallback = async key =>
            //// {
            ////     var connBuilder = await SqlConnectionConfig.ConnectionBuilderCallback(key).ConfigureAwait(false);
            ////     //// Update the connection if required. E.g.:
            ////     // connBuilder.UserId = "sqluser";
            ////     // connBuilder.Password = KeyVaultHelper.GetKeyVaultSecretAsync("SqlClientPwd");
            ////     return connBuilder;
            //// };
            await InitializeAsync().ConfigureAwait(false);

            //// // Uncomment the below line to validate Styles for your Web-pages
            //// ElementStyles = (await new YamlDataSource().ReadAsync<IEnumerable<ElementStyle>>(input: @"TestData\Input\ElementStyles.yml").ConfigureAwait(false)).ToList();
        }

        /// <summary>
        /// Assembly Unload code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterTestRun(Order = Order)]
        public static async Task AssemblyUnload()
        {
            await TeardownAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// After the class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature(Order = Order)]
        public static async Task AfterClass()
        {
            await AfterFeature().ConfigureAwait(false);
        }

        /// <summary>
        /// Before the class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature(Order = Order)]
        public static async Task BeforeClass()
        {
            await BeforeFeature().ConfigureAwait(false);
        }

        /// <summary>
        /// After the test.
        /// </summary>
        /// <param name="featureContext">Feature Context.</param>
        /// <param name="scenarioContext">Scenario Context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [After(Order = Order)]
        public async Task AfterTest(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            await this.AfterScenario(featureContext, scenarioContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Before the test.
        /// </summary>
        /// <param name="featureContext">Feature Context.</param>
        /// <param name="scenarioContext">Scenario Context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Before(Order = Order)]
        public async Task BeforeTest(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            await this.BeforeScenario(featureContext, scenarioContext).ConfigureAwait(false);
        }
    }
}