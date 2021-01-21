using System.Net;
using System.Net.Http.Headers;
using System.Configuration;
using Dapper;
using System.Data.SqlClient;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Campaign HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    string htmlResponse = string.Empty; 

    // parse query parameter
    string campaignId = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "campaignId", true) == 0)
        .Value;

    string userId = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "userId", true) == 0)
        .Value;

    var cnnString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;

    using (var conn = new SqlConnection(cnnString))
    {
        await conn.OpenAsync();
        var sql = "SELECT * FROM [dbo].[Campaign] WHERE Id = @CampaignId;";
        var campaign = (await conn.QueryAsync<Campaign>(sql, new { CampaignId = campaignId })).FirstOrDefault();
        htmlResponse = BuildHtmlResponse(campaign);
    }

    var response = new HttpResponseMessage(HttpStatusCode.OK);
    response.Content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(htmlResponse));
    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

    return response;
}

private static string BuildHtmlResponse(Campaign campaign)
{
    var marketingStorageUri = ConfigurationManager.AppSettings["MarketingStorageUri"];

    return string.Format(@"
      <html>
      <head>
        <link href='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css' rel='stylesheet'>
      </head>
      <header>
        <title>Campaign Details</title>
      </header>
      <body>
        <div class='container'>
          </br>
          <div class='card-deck'>
            <div class='card text-center'>
              <img class='card-img-top' src='{0}' alt='Card image cap'>
              <div class='card-block'>
                <h4 class='card-title'>{1}</h4>
                <p class='card-text'>{2}</p>
                <div class='card-footer'>
                  <small class='text-muted'>From {3} until {4}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </body>
      </html>",
      $"{marketingStorageUri}{campaign.PictureName}",
      campaign.Name,
      campaign.Description,
      campaign.From.ToString("MMMM dd, yyyy"),
      campaign.From.ToString("MMMM dd, yyyy"));
}
 
public class Campaign
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public string PictureUri { get; set; }

    public string PictureName { get; set; }
}
