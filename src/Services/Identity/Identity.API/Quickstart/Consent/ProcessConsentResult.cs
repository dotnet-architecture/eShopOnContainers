// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServerHost.Quickstart.UI;

public class ProcessConsentResult
{
    public bool IsRedirect => RedirectUri != null;
    public string RedirectUri { get; set; }
    public Client Client { get; set; }

    public bool ShowView => ViewModel != null;
    public ConsentViewModel ViewModel { get; set; }

    public bool HasValidationError => ValidationError != null;
    public string ValidationError { get; set; }
}
