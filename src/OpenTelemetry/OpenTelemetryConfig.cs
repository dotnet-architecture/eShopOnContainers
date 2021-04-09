using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Customization
{
    public class OpenTelemetryConfig
    {
        public string ServiceName { get; set; }

        public string ExportType { get; set; }

        public string ExportToolEndpoint { get; set; }

    }
}
