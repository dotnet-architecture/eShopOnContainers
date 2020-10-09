using MediatR;
using System.Runtime.Serialization;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Commands
{
    public class UpdatePicCommand: IRequest<bool>
    {
        public UpdatePicCommand()
        {

        }
        public UpdatePicCommand(string pictureFileName, int catalogItemId)
        {
            CatalogItemId = catalogItemId;
            PictureFileName = pictureFileName;
        }
        [DataMember]
        public int CatalogItemId { get; private set; }
        [DataMember]
        public string PictureFileName { get; private set; }
    }
}
